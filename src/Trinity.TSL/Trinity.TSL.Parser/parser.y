%{
    #include "debug.h"
%}

%code requires{
    #include "SyntaxNode.h"
    #include "error.h"
    #include "flex_bison_common.h"
    #include <string>
    using namespace std;

    #pragma warning(disable:4065) // switch statement contains 'default' but no 'case' labels
}
%locations
%code {
    NTSL        *tsl = NULL;
    YYLTYPE     lhs_loc;//To be captured in YYLLOC_DEFAULT
    #ifdef YYPRINT
    void yy_print_trap(FILE*, unsigned short, YYSTYPE);
    #endif

//	Deprecated stuff
//		index: T_INDEX T_IDENTIFIER T_LCURLY T_RCURLY
//  		                    { $$ = new NIndex(); $$->name = $2;/* TODO */}
//  		;

//  		%type   <indexDescriptor>   index
//  		|index              { tsl = new NTSL(); tsl->indexList->push_back($1);$$=tsl; }
//  		    NIndex                  *indexDescriptor;
//  		|tsl index          { $1->indexList->push_back($2); }

//  		 T_FIXEDMODIFIER    { $$ = $1; }
//  		|T_ELASTICMODIFIER  { $$ = $1; }
// T_EXTERNMODIFIER T_INVISIBLEMODIFIER
// T_EXTERNMODIFIER   { $$ = $1; }
//|T_INVISIBLEMODIFIER{ $$ = $1; }
    

}

%union {
    /* Terminal types */
    int                     token;
    int                     integer;
    std::string             *string;

    /* Key value types */
    vector<std::string*>        *stringList;
    NKVPair                     *kvpair;
    vector<NKVPair*>            *kvlist;

    /* Field types */
    NFieldType                  *fieldType;
    NField                      *field;
    vector<NField*>             *fieldList;
    vector<int>                 *modifierList;

    /* Protocol types */
    NProtocolProperty           *protocolProperty;
    NProtocolReference          *protocolReference;
    vector<NProtocolProperty*>  *proto_prop_list;
    vector<NProtocolReference*> *proto_ref_list;

    /* Enum types */
    NEnumEntry                  *enumEntry;
    vector<NEnumEntry*>         *enumEntryList;

    /* Top-level elements */
    NStruct                 *structDescriptor;
    NCell                   *cellDescriptor;
    NTrinitySettings        *settingsDescriptor;
    NProtocol               *protocolDescriptor;
    NProxy                  *proxyDescriptor;
    NServer                 *serverDescriptor;
    NModule                 *moduleDescriptor;
    NEnum                   *enumDescriptor;

    /* Root object */
    NTSL       *tsl;
}

/* Terminal tokens */
    /* Top-level keywords */
%token	<token>				T_INCLUDE T_TRINITY_SETTINGS T_STRUCT T_CELL T_PROTOCOL T_SERVER T_PROXY T_MODULE T_ENUM 
    /* Data types */
%token	<token>				T_BYTETYPE T_SBYTETYPE T_BOOLTYPE T_CHARTYPE T_SHORTTYPE T_USHORTTYPE T_INTTYPE T_UINTTYPE 
%token	<token>				T_LONGTYPE T_ULONGTYPE T_FLOATTYPE T_DOUBLETYPE T_DECIMALTYPE T_DATETIMETYPE T_GUIDTYPE T_U8STRINGTYPE T_STRINGTYPE T_LISTTYPE T_ARRAYTYPE
    /* Punctations */
%token  <token>             T_LCURLY T_RCURLY T_LSQUARE T_RSQUARE T_SEMICOLON T_COMMA T_COLON T_EQUAL T_SHARP T_LANGLE T_RANGLE T_LPAREN T_RPAREN
    /* Modifiers */
%token	<token>				T_OPTIONALMODIFIER 
    /* Protocol symbols */
%token	<token>				T_TYPE T_SYNCRPC T_ASYNCRPC T_HTTP T_REQUEST T_RESPONSE T_STREAM T_VOID
    /* Values */
%token  <integer>           T_INTEGER
%token  <string>            T_STRING T_STRING_UNCLOSED T_GUIDVALUE T_IDENTIFIER
    /* Comments */			T_COMMENT_LINE T_COMMENT_BLOCK T_COMMENT_BLOCK_UNCLOSED

/* Non-terminal tokens */
%type   <tsl>               tsl

%type   <structDescriptor>  struct
%type   <cellDescriptor>    cell
%type   <settingsDescriptor>settings
%type   <protocolDescriptor>protocol
%type   <proxyDescriptor>   proxy
%type   <serverDescriptor>  server
%type   <moduleDescriptor>  module
%type   <enumDescriptor>    enum

%right T_STRING T_COLON

%type   <kvpair>            kvpair
%type   <kvlist>            settings_kvlist

    /* attributes:          all attributes of a block/field */
    /* attributes_group:    attributes of a block/field in a single [ ... ]*/

%type   <kvlist>            attributes
%type   <kvlist>            attributes_group
%type   <kvlist>            attributes_kvlist

%type   <fieldType>         field_type
%type   <field>             field
%type   <fieldList>         field_list
%type   <token>             modifier
%type   <modifierList>      modifier_list
%type   <token>             atom_type
%type   <stringList>        array_dimension_list

%type   <protocolProperty>  protocol_property
%type   <protocolReference> protocol_reference
%type   <proto_prop_list>   protocol_property_list
%type   <proto_ref_list>    protocol_ref_list

%type   <enumEntry>         enum_entry
%type   <enumEntry>         enum_last_entry
%type   <enumEntryList>     enum_entry_list

/* Pre-processing tokens */
%type   <string>            include

/* Error capturing non-terminals */
%type   <token>              error_top_tier
%type   <token>              error_block_end
%type   <token>              error_stmt_end
%type   <token>              error_toomany_name

%type   <token>              error_struct_no_name
%type   <token>              error_cell_no_name
%type   <token>              error_cell_expect_struct
%type   <token>              error_protocol_no_name
%type   <token>              error_server_no_name
%type   <token>              error_module_no_name
%type   <token>              error_proxy_no_name
%type   <token>              error_enum_no_name

%left T_IDENTIFIER error
%type   <token>              error_field_list
%type   <token>              error_fl_no_field_type
%type   <token>              error_fl_no_name
%type   <token>              error_fl_toomany_name

%type   <token>              error_protocol_ref_list
%type   <token>              error_prl_no_name
%type   <token>              error_prl_no_protocol_keyword
%type   <token>              error_prl_toomany_name
%type   <token>              error_protocol_message_type_specifier

%type   <token>              error_settings_list

%type   <token>              error_protocol_property_list
%type   <token>              error_ppl_type_kw
%type   <token>              error_ppl_type_specifier
%type   <token>              error_ppl_msg
%type   <token>              error_ppl_toomany_name

%type   <token>              error_enum_entry_list
%type   <token>              error_eel_no_name
%type   <token>              error_eel_invalid_value
%type   <token>              error_eel_expect_value

%start  tsl

%%

tsl:
/* blank */         { tsl = new NTSL(); $$ = tsl; }
|tsl struct         { $1->structList->push_back($2); }
|tsl cell           { $1->cellList->push_back($2); }
|tsl settings       { $1->settingsList->push_back($2); }
|tsl protocol       { $1->protocolList->push_back($2); }
|tsl proxy          { $1->proxyList->push_back($2); }
|tsl server         { $1->serverList->push_back($2); }
|tsl module         { $1->moduleList->push_back($2); }
|tsl enum           { $1->enumList->push_back($2); }
/* XXX this include pattern will disrupt the sourceLocation in the
 * TSL. Do not use sourceLocation in NTSL nodes! */
|tsl include        { }
|tsl error_top_tier { }
;

/* Top-tier elements */

include: 
 T_INCLUDE T_STRING T_SEMICOLON
                    {push_new_file($2);};
                    /* BUG: look-ahead at EOF will cause flex to call yyterminate.
                       Thus bison receives the terminate and the last action(pushing the new file)
                        will never be effective.
                       Solution is to disable the rule below so that
                       there's no look-ahead to determine whether
                       there should be a T_SEMICOLON while there isn't
                       (and causing yyterminate)
|T_INCLUDE T_STRING
                    {push_new_file($2);};
                    */

struct: attributes T_STRUCT T_IDENTIFIER T_LCURLY field_list T_RCURLY
                    { $$ = new NStruct(); $$->fieldList = $5; $$->attributes = $1; $$->name = $3;}
;

cell: 
 attributes T_CELL T_IDENTIFIER T_LCURLY field_list T_RCURLY
                    { $$ = new NCell(); $$->fieldList = $5; $$->attributes = $1; $$->name = $3;}
                    /* Alias 'cell struct' */
|attributes T_CELL T_STRUCT T_IDENTIFIER T_LCURLY field_list T_RCURLY
                    { $$ = new NCell(); $$->fieldList = $6; $$->attributes = $1; $$->name = $4;}
;

settings: 
 T_TRINITY_SETTINGS T_IDENTIFIER T_LCURLY settings_kvlist T_RCURLY
                    { $$ = new NTrinitySettings(); $$->name = $2; $$->settings = $4; }
|T_TRINITY_SETTINGS T_LCURLY settings_kvlist T_RCURLY
                    { $$ = new NTrinitySettings(); $$->name = new std::string(); $$->settings = $3; }
;

protocol: T_PROTOCOL T_IDENTIFIER T_LCURLY protocol_property_list T_RCURLY
                    { $$ = new NProtocol(); $$->name = $2; $$->protocolPropertyList = $4; }
;

proxy: T_PROXY T_IDENTIFIER T_LCURLY protocol_ref_list T_RCURLY
                    { $$ = new NProxy(); $$->name = $2; $$->protocolList = $4; }
;

server: T_SERVER T_IDENTIFIER T_LCURLY protocol_ref_list T_RCURLY
                    { $$ = new NServer(); $$->name = $2; $$->protocolList = $4; }
;

module: T_MODULE T_IDENTIFIER T_LCURLY protocol_ref_list T_RCURLY
                    { $$ = new NModule(); $$->name = $2; $$->protocolList = $4; }
;

enum: 
 T_ENUM T_IDENTIFIER T_LCURLY enum_entry_list T_RCURLY
                    { $$ = new NEnum(); $$->name = $2; $$->enumEntryList = $4; }
|T_ENUM T_IDENTIFIER T_LCURLY enum_entry_list  enum_last_entry T_RCURLY
                    { $$ = new NEnum(); $$->name = $2; $$->enumEntryList = $4; $$->enumEntryList->push_back($5); }
;


field_list:
/* blank */         {$$ = new vector<NField*>(); @$.from_blank_rule = true;}
|field_list field
                    {$1->push_back($2);}
|field_list error_field_list
;

attributes:
 /* blank */        {$<kvlist>$ = new vector<NKVPair*>(); @$.from_blank_rule = true;}
|attributes attributes_group
                    {$1->insert($1->end(), $2->begin(), $2->end()); delete $2; $$ = $1;}
;

attributes_group:
 T_LSQUARE T_RSQUARE
                    {$<kvlist>$ = new vector<NKVPair*>(); }
|T_LSQUARE attributes_kvlist T_RSQUARE
                    {$$ = $<kvlist>2;}
;

settings_kvlist: 
 /* blank */        {$$ = new vector<NKVPair*>(); @$.from_blank_rule = true;}
|settings_kvlist kvpair T_SEMICOLON
                    {$<kvlist>1->push_back($2);}
|settings_kvlist error_settings_list
;

protocol_property_list:
/* blank */         {$$ = new vector<NProtocolProperty*>();@$.from_blank_rule = true;}
|protocol_property_list protocol_property
                    {$<proto_prop_list>1->push_back($2);}
|protocol_property_list error_protocol_property_list
;

protocol_reference: T_PROTOCOL T_IDENTIFIER T_SEMICOLON
                    { $$ = new NProtocolReference(); $<protocolReference>$->name = $2;}

protocol_ref_list:
/* blank */         { $$ = new vector<NProtocolReference*>(); @$.from_blank_rule = true;}
|protocol_ref_list protocol_reference
                    { $1->push_back($2);}
|protocol_ref_list error_protocol_ref_list
;

field:
 attributes modifier_list field_type T_IDENTIFIER T_SEMICOLON
                    { $$ = new NField();
                      $$->attributes = $1;
                      $$->modifiers = $2;
                      $$->fieldType = $3;
                      $$->name = $4;
                      $3->field = $$;
                    };

attributes_kvlist:
 kvpair             { $$ = new vector<NKVPair*>(); $$->push_back($1); }
|attributes_kvlist T_COMMA kvpair
                    { $1->push_back($3); }
;

kvpair: 
 kvpair T_COLON T_STRING
                    { $1->value->append(*$3); delete $3; }
|T_STRING T_COLON T_STRING
                    { $$ = new NKVPair(); $$->key = $1; $$->value = $3; }
|T_STRING
                    { $$ = new NKVPair(); $$->key = $1; $$->value = new std::string(""); }
;

protocol_property:
 T_TYPE T_COLON T_SYNCRPC T_SEMICOLON
                    { $$ = new NProtocolProperty(); $$->propertyType = PT_SYN; }
|T_TYPE T_COLON T_ASYNCRPC T_SEMICOLON
                    { $$ = new NProtocolProperty(); $$->propertyType = PT_ASYN;}
|T_TYPE T_COLON T_HTTP T_SEMICOLON
                    { $$ = new NProtocolProperty(); $$->propertyType = PT_HTTP;}
|T_REQUEST T_COLON T_IDENTIFIER T_SEMICOLON
                    { $$ = new NProtocolProperty(); $$->propertyType = PT_STRUCT_REQUEST; $$->data = $3;}
|T_REQUEST T_COLON T_STREAM T_SEMICOLON
                    { $$ = new NProtocolProperty(); $$->propertyType = PT_STREAM_REQUEST;}
|T_REQUEST T_COLON T_VOID T_SEMICOLON
                    { $$ = new NProtocolProperty(); $$->propertyType = PT_VOID_REQUEST;}
|T_RESPONSE T_COLON T_IDENTIFIER T_SEMICOLON
                    { $$ = new NProtocolProperty(); $$->propertyType = PT_STRUCT_RESPONSE; $$->data = $3;}
|T_RESPONSE T_COLON T_STREAM T_SEMICOLON
                    { $$ = new NProtocolProperty(); $$->propertyType = PT_STREAM_RESPONSE;}
|T_RESPONSE T_COLON T_VOID T_SEMICOLON
                    { $$ = new NProtocolProperty(); $$->propertyType = PT_VOID_RESPONSE;}
;

modifier:
 T_OPTIONALMODIFIER { $$ = $1; }
;

modifier_list:
 /* blank */        { $$ = new vector<int>(); @$.from_blank_rule = true;}
|modifier_list modifier
                    { $<modifierList>1->push_back($2); }
;

field_type:
 atom_type          { $$ = new NFieldType(); $$->fieldType = FT_ATOM; $$->atom_token = $1; }
|T_IDENTIFIER       { $$ = new NFieldType(); $$->fieldType = FT_REFERENCE; $$->referencedTypeName = $1; }
|field_type T_LSQUARE array_dimension_list T_RSQUARE
                    { $$ = new NFieldType();
                      $$->fieldType = FT_ARRAY;
                      $$->arrayInfo.arrayElement = $1;
                      $$->arrayInfo.array_dimension_list = $3; 
                      $1->field = NULL;}
|T_ARRAYTYPE T_LANGLE field_type T_RANGLE T_LPAREN array_dimension_list T_RPAREN
                    { $$ = new NFieldType();
                      $$->fieldType = FT_ARRAY;
                      $$->arrayInfo.arrayElement = $3;
                      $$->arrayInfo.array_dimension_list = $6; 
                      $3->field = NULL;}
|T_LISTTYPE T_LANGLE field_type T_RANGLE
                    { $$ = new NFieldType();
                      $$->listElementType = $3;
                      $$->fieldType = FT_LIST; 
                      $3->field = NULL; } ;

atom_type:
 T_BYTETYPE         { $$ = $1; }
|T_SBYTETYPE        { $$ = $1; }
|T_BOOLTYPE         { $$ = $1; }
|T_CHARTYPE         { $$ = $1; }
|T_SHORTTYPE        { $$ = $1; }
|T_USHORTTYPE       { $$ = $1; }
|T_INTTYPE          { $$ = $1; }
|T_UINTTYPE         { $$ = $1; }
|T_LONGTYPE         { $$ = $1; }
|T_ULONGTYPE        { $$ = $1; }
|T_FLOATTYPE        { $$ = $1; }
|T_DOUBLETYPE       { $$ = $1; }
|T_DECIMALTYPE      { $$ = $1; }
|T_DATETIMETYPE     { $$ = $1; }
|T_GUIDTYPE         { $$ = $1; }
|T_STRINGTYPE       { $$ = $1; }
|T_U8STRINGTYPE     { $$ = $1; }
;

array_dimension_list:
 T_STRING           { $<stringList>$ = new std::vector<std::string*>(); 
                      $<stringList>$->push_back($1); }
|array_dimension_list T_COMMA T_STRING
                    { $1->push_back($3); }
;

enum_entry_list:
 /* blank */        {$<enumEntryList>$ = new vector<NEnumEntry*>(); @$.from_blank_rule = true;}
|enum_entry_list enum_entry
                    {$<enumEntryList>1->push_back($2);}
|enum_entry_list error_enum_entry_list
;

enum_entry:         
 T_IDENTIFIER T_EQUAL T_INTEGER T_COMMA
                    {$<enumEntry>$ = new NEnumEntry(); $<enumEntry>$->name = $1; 
                     $<enumEntry>$->value = $3; $<enumEntry>$->value_assigned = true;}
|T_IDENTIFIER T_COMMA
                    {$<enumEntry>$ = new NEnumEntry(); $<enumEntry>$->name = $1; 
                     $<enumEntry>$->value = 0; $<enumEntry>$->value_assigned = false;}
;
enum_last_entry:    
 T_IDENTIFIER T_EQUAL T_INTEGER
                    {$<enumEntry>$ = new NEnumEntry(); $<enumEntry>$->name = $1; 
                     $<enumEntry>$->value = $3; $<enumEntry>$->value_assigned = true;}
|T_IDENTIFIER
                    {$<enumEntry>$ = new NEnumEntry(); $<enumEntry>$->name = $1; 
                     $<enumEntry>$->value = 0; $<enumEntry>$->value_assigned = false;}
;

    /* Error handling paths */

error_block_end:            error T_RCURLY                              {$$ = 0;};
error_stmt_end:             error T_SEMICOLON                           {$$ = 0;};
error_toomany_name:         T_IDENTIFIER T_IDENTIFIER error T_SEMICOLON {$$ = 0;};

error_top_tier: 
 error_struct_no_name       {error(@1, "struct: expecting an identifier.");}
|error_cell_no_name         {error(@1, "cell: expecting an identifier.");}
|error_cell_expect_struct   {error(@1, "cell: expecting keyword \"struct\".");}
|error_protocol_no_name     {error(@1, "protocol: expecting an identifier.");}
|error_server_no_name       {error(@1, "server: expecting an identifier.");}
|error_module_no_name       {error(@1, "module: expecting an identifier.");}
|error_proxy_no_name        {error(@1, "proxy: expecting an identifier.");}
|error_enum_no_name         {error(@1, "enum: expecting an identifier.");}
;

error_struct_no_name:       attributes  T_STRUCT        error_block_end                 {$$ = $2;};
error_cell_no_name:         attributes  T_CELL          T_STRUCT        error_block_end {$$ = $2;};
error_cell_expect_struct:   attributes  T_CELL          error_block_end                 {$$ = $2;};
error_protocol_no_name:     T_PROTOCOL  error_block_end                                 ;
error_server_no_name:       T_SERVER    error_block_end                                 ;
error_module_no_name:       T_MODULE    error_block_end                                 ;
error_proxy_no_name:        T_PROXY     error_block_end                                 ;
error_enum_no_name:         T_ENUM      error_block_end                                 ;

error_field_list:
 error_fl_no_field_type     {error(@1, "field: type not specified.");}
|error_fl_no_name           {error(@1, "field: expecting an identifier.");}
|error_fl_toomany_name      {error(@1, "field: too many identifiers.");}
|error_stmt_end             {error(@1, "field: syntax error.");}
;


error_fl_no_field_type:     attributes modifier_list T_IDENTIFIER error_stmt_end        {$$ = 0;};
error_fl_no_name:           attributes modifier_list field_type error_stmt_end          {$$ = 0;};
error_fl_toomany_name:      attributes modifier_list field_type error_toomany_name      {$$ = 0;};

error_protocol_ref_list:
 error_prl_no_name          {error(@1, "protocol references: specify a protocol.");}
|error_prl_no_protocol_keyword
                            {error(@1, "protocol references: expecting a \"protocol\" keyword.");}
|error_prl_toomany_name     {error(@1, "protocol references: too many identifiers.");}
;

error_prl_no_name:          T_PROTOCOL error_stmt_end                   ;
error_prl_no_protocol_keyword: error_stmt_end                           ;
error_prl_toomany_name:     T_PROTOCOL error_toomany_name               ;

error_settings_list:        
 error_stmt_end             {error(@1, "settings: specify settings with format \"key:value;\".");}
;

error_protocol_property_list:
 error_ppl_type_kw          {error(@1, "protocol: expecting keyword \"type\".");}
|error_ppl_type_specifier   {error(@1, "protocol: expecting type specifier (syn/asyn).");}
|error_ppl_msg              {error(@1, "protocol: expecting message type.");}
|error_ppl_toomany_name     {error(@1, "protocol: too many message type specifiers.");}
|error_stmt_end             {error(@1, "protocol: syntax error.");}
;

error_ppl_type_kw:          error       T_COLON         T_SYNCRPC       {$$=0;} | 
                            error       T_COLON         T_ASYNCRPC      {$$=0;} |
                            error       T_COLON         T_HTTP          {$$=0;};
error_ppl_type_specifier:   T_TYPE      T_COLON         error_stmt_end;
error_ppl_msg:              T_REQUEST   error_stmt_end                          | T_RESPONSE    error_stmt_end;
error_ppl_toomany_name:     T_REQUEST   T_COLON
                            error_protocol_message_type_specifier error_protocol_message_type_specifier error T_SEMICOLON
                      |     T_RESPONSE  T_COLON
                            error_protocol_message_type_specifier error_protocol_message_type_specifier error T_SEMICOLON
;

error_protocol_message_type_specifier:
 T_VOID
|T_STREAM
|T_IDENTIFIER {$$ = 0;}
;


error_enum_entry_list:
 error_eel_no_name          {error(@1, "enum: epxecting an identifier.");}
|error_eel_invalid_value    {error(@1, "enum: invalid value.");}
|error_eel_expect_value     {error(@1, "enum: expecting a value, or ','.");}

error_eel_no_name:          error        T_COMMA                            {$$=0;};
error_eel_invalid_value:    T_IDENTIFIER T_EQUAL    error   T_COMMA         {$$=0;};
error_eel_expect_value:     T_IDENTIFIER error      T_COMMA                 {$$=0;};

