public enum PrologoState
{
    Inicio,                 // Narrador falando, player chegando no lab
    ConversarCientistas,    // Precisa falar com todos antes do puzzle
    PodeResolverPuzzle,     // Falou com todos, máquina liberada
    PuzzleResolvido,        // Acabou o puzzle, precisa falar de novo
    ProntoParaViajar        // Falou com todos, pode ir para Fase 2
}

public enum Dia1State
{
    InvestigarTaverna,  // Precisa falar com Taberneiro + Bêbado
    FalarComEscravo,    // Ir ao armazém
    MensageiroFugindo,  // O NPC corre pelo fundo até o porto
    EscolhaFinal,       // No porto, escolhe a carta
    Vitoria,            // Narrador -> Dia 2
    GameOver            // Escolheu errado
}