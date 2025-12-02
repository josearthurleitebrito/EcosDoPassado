using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class WireManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject puzzlePanelUI; // Arraste o objeto PAI (WireTaskRoot) aqui também

    // ... (Copie as variáveis de Anchors, Prefabs, Settings do seu código antigo) ...
    [Header("Anchors")]
    public Transform[] leftAnchors;
    public Transform[] rightAnchors;
    [Header("Prefabs")]
    public GameObject startNodePrefab;
    public GameObject endNodePrefab;
    public GameObject wireLinePrefab;
    [Header("Settings")]
    public Color[] wireColors = { Color.red, Color.blue, Color.green, new Color(1f, 0.85f, 0f) };
    public SpriteRenderer completeLight; 
    public bool randomizeRows = true;

    [HideInInspector] public Camera cam;

    private readonly List<WirePair> pairs = new List<WirePair>();
    private int connectedCount = 0;
    private WireHandle activeHandle;
    private bool isPuzzleSolved = false;

    void Awake()
    {
        cam = Camera.main;
        if (cam == null) Debug.LogWarning("[WireManager] Nenhuma Camera Main encontrada.");
    }

  [System.Obsolete]
  void OnEnable()
    {
        // Reseta o puzzle ao abrir
        BuildPuzzle();
        isPuzzleSolved = false;
        
        // Opcional: Travar o player enquanto resolve
        var player = FindObjectOfType<PlayerController>();
        if (player) player.enabled = false;
    }

  [System.Obsolete]
  void OnDisable()
    {
        // Destravar o player ao fechar
        var player = FindObjectOfType<PlayerController>();
        if (player) player.enabled = true;
    }

    void EnsureCamera()
    {
        if (cam == null) cam = Camera.main;
    }

    void BuildPuzzle()
    {
        // Limpeza
        foreach (var p in pairs)
        {
            if (p.start) Destroy(p.start.gameObject);
            if (p.end) Destroy(p.end.gameObject);
            if (p.line) Destroy(p.line.gameObject);
        }
        pairs.Clear();
        connectedCount = 0;
        if (completeLight) completeLight.gameObject.SetActive(false);

        // Validação
        if (leftAnchors == null || leftAnchors.Length < 4 || rightAnchors == null || rightAnchors.Length < 4) return;
        if (!startNodePrefab || !endNodePrefab || !wireLinePrefab) return;

        // Embaralhamento
        List<int> indices = new List<int> { 0, 1, 2, 3 };
        if (randomizeRows)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                int j = Random.Range(i, indices.Count);
                (indices[i], indices[j]) = (indices[j], indices[i]);
            }
        }

        // Criação dos fios
        for (int k = 0; k < 4; k++)
        {
            var startGO = Instantiate(startNodePrefab, leftAnchors[k].position, Quaternion.identity, transform);
            var startHandle = startGO.GetComponent<WireHandle>();
            if (startHandle == null) startHandle = startGO.AddComponent<WireHandle>();

            int rIndex = indices[k];
            var endGO = Instantiate(endNodePrefab, rightAnchors[rIndex].position, Quaternion.identity, transform);
            var endNode = endGO.GetComponent<WireEnd>();
            if (endNode == null) endNode = endGO.AddComponent<WireEnd>();

            var lineGO = Instantiate(wireLinePrefab, Vector3.zero, Quaternion.identity, transform);
            var bezier = lineGO.GetComponent<BezierWire>();
            if (bezier == null) bezier = lineGO.AddComponent<BezierWire>();

            Color c = wireColors[k % wireColors.Length];

            startHandle.Init(this, k, c, bezier);
            endNode.Init(this, k, c);

            var pair = new WirePair { index = k, start = startHandle, end = endNode, line = bezier };
            pairs.Add(pair);

            bezier.SetStaticPoints(startGO.transform, startGO.transform);
            bezier.SetColor(c);
        }
    }

    void Update()
    {
        if (isPuzzleSolved) return;

        EnsureCamera();
        if (cam == null) return;

        // Input: Down
        if (PointerDownThisFrame())
        {
            Vector2 mp = GetPointerWorldPosition();
            var hit = Physics2D.OverlapPoint(mp, ~0);
            if (hit != null)
            {
                var handle = hit.GetComponent<WireHandle>();
                if (handle != null)
                {
                    activeHandle = handle;
                    activeHandle.BeginDrag();
                }
            }
        }

        // Input: Drag & Up
        if (activeHandle != null)
        {
            Vector3 mp3 = GetPointerWorldPosition();
            mp3.z = 0f;
            activeHandle.DragTo(mp3);

            if (PointerUpThisFrame())
            {
                Vector2 mp = mp3;
                var hits = Physics2D.OverlapPointAll(mp, ~0);
                WireEnd target = null;
                foreach (var h in hits)
                {
                    var e = h.GetComponent<WireEnd>();
                    if (e != null) { target = e; break; }
                }

                if (target != null)
                {
                    bool ok = TryConnect(activeHandle.wireIndex, target);
                    if (!ok) ResetLineToStart(activeHandle.wireIndex);
                }
                else
                {
                    ResetLineToStart(activeHandle.wireIndex);
                }

                activeHandle.EndDrag();
                activeHandle = null;
            }
        }
    }

    public bool TryConnect(int wireIndex, WireEnd dropOn)
    {
        if (dropOn.wireIndex == wireIndex && !dropOn.isOccupied)
        {
            var pair = pairs[wireIndex];
            dropOn.isOccupied = true;
            pair.start.Lock();
            pair.line.SnapTo(pair.start.transform, dropOn.transform);
            connectedCount++;

            // --- VITÓRIA ---
            if (connectedCount >= pairs.Count)
            {
                if (completeLight) completeLight.gameObject.SetActive(true);
                Debug.Log("[WireManager] PUZZLE VENCIDO!");
                StartCoroutine(VictoryRoutine());
            }
            return true;
        }
        return false;
    }

    // --- AQUI ESTAVA O ERRO, AGORA CORRIGIDO ---
    IEnumerator VictoryRoutine()
    {
        isPuzzleSolved = true;
        yield return new WaitForSeconds(1.5f); // Pequena pausa para ver a luz acesa

        // Fecha o painel
        if (puzzlePanelUI != null) puzzlePanelUI.SetActive(false);

        // Chama o PrologueManager com a função NOVA
        if (FaseManager.Instance != null)
        {
            // CORREÇÃO: Usar NotifyPuzzleSolved em vez de TriggerPostPuzzle
            //FaseManager.Instance.NotifyPuzzleSolved();
        }
        else
        {
            Debug.LogError("PrologueManager não encontrado na cena!");
        }
    }

    public void ResetLineToStart(int wireIndex)
    {
        if (wireIndex < 0 || wireIndex >= pairs.Count) return;
        var pair = pairs[wireIndex];
        pair.line.SnapTo(pair.start.transform, pair.start.transform);
        pair.start.Unlock();
    }

    // Input Helpers
    bool PointerDownThisFrame()
    {
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) ||
               (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);
        #else
        return Input.GetMouseButtonDown(0);
        #endif
    }

    bool PointerUpThisFrame()
    {
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame) ||
               (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame);
        #else
        return Input.GetMouseButtonUp(0);
        #endif
    }

    Vector3 GetPointerWorldPosition()
    {
        Vector2 screenPos = Input.mousePosition;
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        else if (Mouse.current != null)
            screenPos = Mouse.current.position.ReadValue();
        #endif
        Vector3 wp = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(cam.transform.position.z)));
        wp.z = 0f;
        return wp;
    }

    class WirePair
    {
        public int index;
        public WireHandle start;
        public WireEnd end;
        public BezierWire line;
    }
}