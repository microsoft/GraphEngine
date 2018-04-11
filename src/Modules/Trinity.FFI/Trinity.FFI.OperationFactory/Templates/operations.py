from generate_helper import Method

_ = '{_}'

subject_name = '{subject name}'

object_name = '{object name}'

object_type = '{object type}'

fn_addr = '{fn addr}'


def _subject_object_fn_name(category: type, verb: type):
    operation_name = verb.__name__
    category_name = category.__name__
    public_fn_name = f'{category_name}{_}{operation_name}{_}{subject_name}{_}{object_name}'
    private_fn_name = f'{_}{public_fn_name}'
    return public_fn_name, private_fn_name


def _subject_fn_name(category: type, verb: type):
    operation_name = verb.__name__
    category_name = category.__name__
    public_fn_name = f'{category_name}{_}{operation_name}{_}{subject_name}'
    private_fn_name = f'{_}{public_fn_name}'
    return public_fn_name, private_fn_name


def index_getter_setter(verb: type):
    @Method
    def call(category: type):
        public_fn_name, private_fn_name = _subject_object_fn_name(category, verb)
        return f"""
        static void (* {private_fn_name})(void* subject, int idx, {object_type} &object) = {fn_addr};
        static void {public_fn_name}(void* subject, int idx, {object_type} &object){{
                {private_fn_name}(subject, idx, object);
        }}
        """

    return call


def getter_setter(verb: type):
    @Method
    def call(category: type):
        public_fn_name, private_fn_name = _subject_object_fn_name(category, verb)
        return f"""
        static void (* {private_fn_name})(void* subject, {object_type} &object) = {fn_addr};
        static void {public_fn_name}(void* subject, {object_type} &object){{
                {private_fn_name}(subject, object);
        }}
        """

    return call


def single_object_method_return(return_type: str):
    def call1(verb: type):
        @Method
        def call2(category: type):
            public_fn_name, private_fn_name = _subject_object_fn_name(category, verb)
            return f"""
            static {return_type} (* {private_fn_name})(void* subject, {object_type} &object) = {fn_addr};
            static {return_type} {public_fn_name}(void* subject, {object_type} &object){{
                    {private_fn_name}(subject, object);
            }}
            """

        return call2

    return call1


def only_subject_method_return(return_type: str):
    def call1(verb: type):
        @Method
        def call2(category: type):
            public_fn_name, private_fn_name = _subject_fn_name(category, verb)
            return f"""
            static {return_type} (* {private_fn_name})(void* subject) = {fn_addr};
            static {return_type} {public_fn_name}(void* subject){{
                    {private_fn_name}(subject);
            }}
            """

        return call2

    return call1
