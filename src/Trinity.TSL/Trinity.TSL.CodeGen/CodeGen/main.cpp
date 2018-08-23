#include <os/os.h>
#include <utilities>
#include <corelib>
#include <io>

#include <iostream>
#include <fcntl.h>

#ifdef TRINITY_PLATFORM_WINDOWS
#include <io.h>
#include <tchar.h>
#endif

#include "Trinity.TSL.CodeGen.h"
#include "Trinity.TSL.Parser.h"

#pragma region Parameters
bool            c_debug;
String          c_namespace;
String 			c_project_root;
std::vector<String>    c_script_list;
String 			c_output_path;
bool            c_delay_sign;
bool            c_no_warnings;
int             c_offset;
#pragma endregion

void help()
{
    Console::WriteLine("Trinity Specification Language Codegen: transpiles TSL scripts ");
    Console::WriteLine("to C# source files for data modeling, message passing modeling,");
    Console::WriteLine("and data interchange for a Graph Engine application.           ");
    Console::WriteLine("                                                               ");
    Console::WriteLine("usage:                                                         ");
    Console::WriteLine("Trinity.TSL.CodeGen.exe [options] [1.tsl 2.tsl ...]            ");
    Console::WriteLine("                                                               ");
    Console::WriteLine("options:                                                       ");
    Console::WriteLine("    -h/--help           Print this help.                       ");
    Console::WriteLine("    -g/--Debug          Enable debugging features.             ");
    Console::WriteLine("    -n/--RootNamespace  Specifies the root namespace.          ");
    Console::WriteLine("                        Defaults to 'Trinity.Extension'.       ");
    Console::WriteLine("    -p/--ProjectRoot    Specifies the root of the project.     ");
    Console::WriteLine("                        Defaults to the current directory.     ");
    Console::WriteLine("    -o/--OutputPath     Specifies the path of the output files.");
    Console::WriteLine("                        Defaults to 'GeneratedCode'            ");
    Console::WriteLine("    -s/--ScriptList     Specifies a list of tsl scripts,       ");
    Console::WriteLine("                        separated by ';'                       ");
    Console::WriteLine("                        Files specified by -s will be processed");
    Console::WriteLine("                        together with other files given in the ");
    Console::WriteLine("                        arguments.                             ");
    Console::WriteLine("    --NoWarning         Suppresses TSL transpiler warnings.    ");
    Console::WriteLine("    --offset            Set TSL CellType offset use an integer.");

}

#ifdef TRINITY_PLATFORM_WINDOWS
bool get_parameters(int argc, u16char** argv)
#else
bool get_parameters(int argc, char** argv)
#endif
{
    auto args                                      = CommandLineTools::GetArguments(argc, argv);
    auto BuildDataModelingProjectWithDebugFeatures = CommandLineTools::DefineOption<bool>("g", "Debug");
    auto Namespace                                 = CommandLineTools::DefineOption<Trinity::String>("n", "RootNamespace");
    auto ProjectRoot                               = CommandLineTools::DefineOption<Trinity::String>("p", "ProjectRoot");
    auto ScriptFileList                            = CommandLineTools::DefineOption<Trinity::String>("s", "ScriptList"); //TPJ: TSL project
    auto OutputPath                                = CommandLineTools::DefineOption<Trinity::String>("o", "OutputPath");
    auto NoWarning                                 = CommandLineTools::DefineOption<bool>("NoWarning", "NoWarning");
    auto Help                                      = CommandLineTools::DefineOption<bool>("h", "help");
    auto Offset                                    = CommandLineTools::DefineOption<unsigned int>("offset", "offset");

    CommandLineTools::GetOpt(args,
                             BuildDataModelingProjectWithDebugFeatures,
                             Namespace,
                             ProjectRoot,
                             ScriptFileList,
                             OutputPath,
                             NoWarning,
                             Help,
                             Offset);

    if (Help.value)
    {
        help();
        return false;
    }

    c_debug            = BuildDataModelingProjectWithDebugFeatures.set;
    c_namespace        = Namespace.set ? Namespace.value.Trim() : "Trinity.Extension";
    c_project_root     = ProjectRoot.set ? Path::RemoveInvalidChars(ProjectRoot.value.Trim()) : ".";
    c_script_list      = Path::RemoveInvalidChars(ScriptFileList.value.Trim()).Split(";").ToList();
    c_output_path      = OutputPath.set ? Path::RemoveInvalidChars(OutputPath.value.Trim()) : ".";
    c_no_warnings      = NoWarning.set;
    c_offset           = Offset.set ? Offset.value : 0;


    /* Append any other arguments to the file list. */
    for (auto &arg : args)
    {
        if (arg.StartsWith('-'))
        {
            error("Unrecognized parameter: " + arg);
            help();
            return false;
        }
        else
        {
            c_script_list.push_back(Path::RemoveInvalidChars(arg));
        }
    }

    if (!Environment::SetCurrentDirectory(c_project_root))
    {
        error("Cannot switch to directory " + c_project_root);
        return false;
    }

    if (!Path::IsPathRooted(c_output_path))
    {
        c_output_path = Path::Combine(c_project_root, c_output_path);
    }

    if (c_script_list.empty())
    {
        error("No input specified.");
        help();
        return false;
    }

    return true;
}

bool print_parameters()
{
#define OUTPUT_PARAMETER(name, var) do {Console::WriteLine("\t{0:20}= {1}", #name, var);}while(0)
    Console::WriteLine("Compiling with parameters:");
    OUTPUT_PARAMETER(Debug, c_debug);
    OUTPUT_PARAMETER(Namespace, c_namespace);
    OUTPUT_PARAMETER(ProjectRoot, c_project_root);
    OUTPUT_PARAMETER(OutputPath, c_output_path);
    OUTPUT_PARAMETER(CellTypeOffset, c_offset);

    return true;
}

bool parse_syntax_tree(NTSL* &unmanaged_syntax_tree)
{
    Console::WriteLine("Parsing TSL files...");
    if (c_no_warnings) { parser_disable_warnings(); }
    unmanaged_syntax_tree = start_parser(c_script_list);

    return (unmanaged_syntax_tree != NULL);
}

bool generate_source_files(NTSL* syntax_tree)
{
    Console::WriteLine("Generating source files...");
    auto source_output_dir = Path::Combine(c_output_path, "GeneratedCode");
    auto *unmanaged_filelist = Trinity::Codegen::codegen_entry(syntax_tree, source_output_dir, c_namespace, c_offset);
    if (unmanaged_filelist == NULL)
        return false;

    for (auto *file : *unmanaged_filelist)
    {
        delete file;
    }

    //  TODO before porting, we keep track of the generated files (from both old/new codegens)
    //  and send them to the legacy compiler.
    //  Now, as our model has changed to a pure codegen (let msbuild handle the build),
    //  the file list is not used here at the moment, but we keep it here because we
    //  may still need this list in the future, for example, to automatically generate
    //  a CoreCLR project file and compile it.

    bool ret = unmanaged_filelist->size() > 0;
    delete unmanaged_filelist;
    return ret;
}

bool reset_syntax_parser()
{
    reset_parser();
    return true;
}

#define QUIT_ON_ERROR(stmt) do{if(false == stmt)Environment::Exit(-1);}while(0)

#ifdef TRINITY_PLATFORM_WINDOWS
int wmain(int argc, u16char** argv)
#else
int main(int argc, char** argv)
#endif
{
#ifdef TRINITY_PLATFORM_WINDOWS
    _setmode(_fileno(stdout), _O_U8TEXT);
#endif

    NTSL*                                               unmanaged_syntax_tree;

    QUIT_ON_ERROR(get_parameters(argc, argv));
    QUIT_ON_ERROR(print_parameters());
    QUIT_ON_ERROR(parse_syntax_tree(unmanaged_syntax_tree));

    QUIT_ON_ERROR(generate_source_files(unmanaged_syntax_tree));
    QUIT_ON_ERROR(reset_syntax_parser());
    return 0;
}
