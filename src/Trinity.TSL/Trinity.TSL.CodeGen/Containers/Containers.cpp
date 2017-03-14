#include <unordered_set>
#include <corelib>
#include <utilities>
#include <io>
#include "common.h"
#include "Trinity.TSL.CodeGen.h"
#include "Trinity.TSL.Parser.h"

namespace Trinity
{
    namespace Codegen
    {
        std::string* Array(NFieldType*);
        std::string* List(NFieldType*);

        std::string* Containers(NTSL* ntsl)
        {
            std::string* source = new std::string();
            source->append(R"::(using System;
using System.Collections;
using System.Collections.Generic;
using Trinity.Core.Lib;
using Trinity.TSL;
using Trinity.TSL.Lib;
)::");
            for (auto &ft : *TSLExternalParserDataTypeVector)
            {
                std::string* container_code = nullptr;
                if (ft->is_list())
                {
                    auto element = ft->listElementType;
                    if (element->is_atom()) switch (element->atom_token)
                    {
                    case T_BYTETYPE: case T_INTTYPE: case T_LONGTYPE: case T_DOUBLETYPE:
                        /* skip hand-written templates */
                        continue;
                    }
                    container_code = List(ft);
                }
                else if (ft->is_array())
                {
                    container_code = Array(ft);
                }

                source->append(*container_code);
                delete container_code;
            }

            return source;
        }
    }
}

