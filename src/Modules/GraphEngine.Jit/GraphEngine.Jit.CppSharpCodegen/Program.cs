using CppSharp;
using CppSharp.AST;
using CppSharp.Passes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphEngine.Jit.CppSharpCodegen
{
    class AsmJit : ILibrary
    {
        public void Postprocess(Driver driver, ASTContext ctx)
        {
        }

        public void Preprocess(Driver driver, ASTContext ctx)
        {
            int passes_removed = 0;

            //Asmjit already comes with getter/setters, field getter setters will turn to duplicated props
            passes_removed += driver.Context.TranslationUnitPasses.Passes.RemoveAll(_ => _ is FieldToPropertyPass);
            //Incorrectly identifies X86Emitter as static class
            passes_removed += driver.Context.TranslationUnitPasses.Passes.RemoveAll(_ => _ is CheckStaticClass);
            //Duplicated conversion operators
            passes_removed += driver.Context.TranslationUnitPasses.Passes.RemoveAll(_ => _ is ConstructorToConversionOperatorPass);
            //Remove case modifications..
            passes_removed += driver.Context.TranslationUnitPasses.Passes.RemoveAll(_ => _ is CaseRenamePass);
            //Remove case modifications..
            passes_removed += driver.Context.TranslationUnitPasses.Passes.RemoveAll(_ => _ is RenamePass);

            Console.WriteLine($"Removed passes: {passes_removed}.");

            //Rename GraphEngine.Jit.TypeId.IntPtr to GraphEngine.Jit.TypeId.IntPtr_t,
            //to avoid confusion with System.IntPtr.
            var typeIdClass = ctx.FindClass("TypeId").First();
            typeIdClass.FindClass("IntPtr").Name = "IntPtr_t";

            //Rename GraphEngine.Jit.UInt64 to UInt64_t
            var uint64Struct = ctx.FindClass("UInt64").First();
            uint64Struct.Name = "UInt64_t";

            //Eliminate duplicated implicit conversion operators
            var c = ctx.FindClass("X86Compiler").First();
            c.FindOperator(CXXOperatorKind.Conversion).Last().Ignore = true;

            //Turn off bindings for X86InstDB
            var db = ctx.FindClass("X86InstDB").First();
            db.Ignore = true;

            //Turn off bindings for explicit X86Emitter
            var emitter = ctx.FindClass("X86Emitter").First();
            emitter.Ignore = true;

        }

        public void Setup(Driver driver)
        {
            Diagnostics.Level = DiagnosticKind.Message;

            string c(params string[] p) => Path.Combine(p);
            string d(string p) => Path.GetDirectoryName(p);

            var opt = driver.Options;
            opt.GeneratorKind = CppSharp.Generators.GeneratorKind.CSharp;
            opt.GenerateFunctionTemplates = true;
            opt.GenerateClassTemplates = true;
            opt.Quiet = false;
            var mod = opt.AddModule("asmjit");
            var asmpath = d(Assembly.GetExecutingAssembly().Location);
            var projpath = c(asmpath, "..", "..", "..");
            var solpath = c(projpath, "..");

            var asmjitpath = c(solpath, "asmjit");
            var asmjitsrcpath = c(asmjitpath, "src", "asmjit");
            var asmjitbuildpath = c(asmjitpath, "build", "Release");

            mod.IncludeDirs.Add(asmjitsrcpath);
            //mod.Headers.AddRange(Directory.GetFiles(asmjitsrcpath, "*.h", SearchOption.AllDirectories));
            //mod.Headers.Add(c(asmjitsrcpath, "x86", "x86assembler.h"));
            //mod.Headers.Add(c(asmjitsrcpath, "x86", "x86emitter.h"));
            mod.Headers.Add(c(asmjitsrcpath, "x86", "x86compiler.h"));
            mod.Headers.Add(c(asmjitsrcpath, "base", "runtime.h"));

            //mod.Headers.Add(c(projpath, "Templates.h"));

            mod.LibraryDirs.Add(asmjitbuildpath);
            mod.Libraries.Add("asmjit.lib");

            mod.Defines.Add("WIN32");
            mod.Defines.Add("_WINDOWS");
            mod.Defines.Add("NDEBUG");
            mod.Defines.Add("_UNICODE");

            mod.OutputNamespace = "GraphEngine.Jit.Native";
        }

        public void SetupPasses(Driver driver)
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            File.Delete("asmjit.cs");
            File.Delete("Std.cs");
            ConsoleDriver.Run(new AsmJit());
            PatchAsmjit();
            File.Copy("asmjit.cs", "../../../../GraphEngine.Jit.Native/asmjit.cs", overwrite: true);
            File.Copy("Std.cs", "../../../../GraphEngine.Jit.Native/Std.cs", overwrite: true);
        }

        private static void PatchAsmjit()
        {
            var patched_src = File.ReadAllText("asmjit.cs").Replace("protected static global::GraphEngine.Jit.Native.asmjit.X86Compiler _addOptions", "public static global::GraphEngine.Jit.Native.asmjit.X86Compiler _addOptions");
            File.WriteAllText("asmjit.cs", patched_src);
        }
    }
}
