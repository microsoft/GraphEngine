#include <unordered_set>
#include <corelib>
#include <utilities>
#include <io>
#include <set>
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
			std::set<string> initialized_types;
            for (auto &ft : *TSLExternalParserDataTypeVector)
            {
				if (!ft->is_list() && !ft->is_array()) continue;

				auto accessor_name = data_type_get_accessor_name(ft);
				if (initialized_types.find(accessor_name) != initialized_types.end()) continue;
				initialized_types.insert(accessor_name);

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

                if (container_code != nullptr)
                {
                    source->append(*container_code);
                    delete container_code;
                }
            }

            return source;
        }
    }
}

