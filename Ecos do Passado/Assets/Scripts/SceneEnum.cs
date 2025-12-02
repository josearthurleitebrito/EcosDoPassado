public enum PrologoState
{
    Inicio,                 // Narrador falando, player chegando no lab
    ConversarCientistas,    // Precisa falar com todos antes do puzzle
    PodeResolverPuzzle,     // Falou com todos, máquina liberada
    PuzzleResolvido,        // Acabou o puzzle, precisa falar de novo
    ProntoParaViajar        // Falou com todos, pode ir para Fase 2
}