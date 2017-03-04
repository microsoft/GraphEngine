#include <utilities>
#include <corelib>
#include <io>

#include <iostream>
#include <fcntl.h>
#include <io.h>
#include <tchar.h>

#include "Trinity.TSL.CodeGen.h"
#include "Trinity.TSL.Parser.h"
#include <os/os.h>

#pragma region Parameters
bool            c_debug;
String          c_namespace;
String 			c_project_root;
List<String>    c_script_list;
String 			c_output_path;
bool            c_delay_sign;
bool            c_no_warnings;
#pragma endregion

#ifdef TRINITY_PLATFORM_WINDOWS
bool get_parameters(int argc, u16char** argv)
#else
bool get_parameters(int argc, char** argv)
#endif
{
    auto args                                      = CommandLineTools::GetArguments(argc, argv);
    auto BuildDataModelingProjectWithDebugFeatures = CommandLineTools::DefineOption<bool>("g", "BuildDataModelingProjectWithDebugFeatures");
    auto TPJ_Namespace                             = CommandLineTools::DefineOption<Trinity::String>("ns", "RootNamespace");
    auto TSL_ProjectRoot                           = CommandLineTools::DefineOption<Trinity::String>("r", "ProjectRoot");
    auto TPJ_ScriptFileList                        = CommandLineTools::DefineOption<Trinity::String>("ScriptList", "ScriptList"); //TPJ: TSL project
    auto TPJ_OutputPath                            = CommandLineTools::DefineOption<Trinity::String>("p", "OutputPath");
    auto NoWarning                                 = CommandLineTools::DefineOption<bool>("NoWarning", "NoWarning");
    auto TPJ_Reference                             = CommandLineTools::DefineOption<Trinity::String>("r", "Reference");

    CommandLineTools::GetOpt(args,
                             BuildDataModelingProjectWithDebugFeatures,
                             TPJ_Namespace,
                             TSL_ProjectRoot,
                             TPJ_ScriptFileList,
                             TPJ_OutputPath,
                             TPJ_Reference,
                             NoWarning);

    c_debug            = BuildDataModelingProjectWithDebugFeatures.set;
    c_namespace        = TPJ_Namespace.set ? TPJ_Namespace.value.Trim() : "Trinity.Extension";
    c_project_root     = TSL_ProjectRoot.set ? TSL_ProjectRoot.value.Trim() : ".";
    c_script_list      = TPJ_ScriptFileList.value.Trim().Split(";").ToList();
    c_output_path      = TPJ_OutputPath.set ? TPJ_OutputPath.value.Trim() : ".";
    c_no_warnings      = NoWarning.set;

    /* Append any other arguments to the file list. */
    for (auto &arg : args)
    {
        if (arg.StartsWith('-'))
        {
            error("Unrecognized parameter: " + arg); return false;
        }
        else
        {
            //TODO check if arg is a valid filename
            c_script_list.push_back(arg);
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
        // TODO print help
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
    //OUTPUT_PARAMETER(ScriptList, c_script_list);
    OUTPUT_PARAMETER(OutputPath, c_output_path);

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
    auto *unmanaged_filelist = Trinity::Codegen::codegen_entry(syntax_tree, source_output_dir, c_namespace);
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
    _setmode(_fileno(stdout), _O_U8TEXT);

    NTSL*                                               unmanaged_syntax_tree;

    QUIT_ON_ERROR(get_parameters(argc, argv));
    QUIT_ON_ERROR(print_parameters());
    QUIT_ON_ERROR(parse_syntax_tree(unmanaged_syntax_tree));
    QUIT_ON_ERROR(generate_source_files(unmanaged_syntax_tree));
    QUIT_ON_ERROR(reset_syntax_parser());

    return 0;
}