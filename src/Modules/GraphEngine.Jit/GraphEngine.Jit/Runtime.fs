namespace GraphEngine.Jit

module Runtime = 
    open AsmJit
    let private s_rt = Runtime()
    let MakeCompiler() = s_rt.