#define PY_SSIZE_T_CLEAN  /* Make "s#" use Py_ssize_t rather than int. */
#include <Python.h>
#include <Trinity.FFI.Native.h>

extern "C" {
    typedef struct
    {
        PyObject_HEAD
        Cell cell_handle;
    } TrinityCell_Object;

    int64_t trinity_cell_init(TrinityCell_Object *self, PyObject *args, PyObject *kwds);
    void trinity_cell_dealloc(TrinityCell_Object*);
    PyObject* trinity_cell_tostring(TrinityCell_Object*);

    static PyTypeObject trinity_cellType = {
        PyVarObject_HEAD_INIT(NULL, 0) 
        "trinity.cell", /* tp_name */
        sizeof(TrinityCell_Object),                    /* tp_basicsize */
        0,                                             /* tp_itemsize */
        (destructor)trinity_cell_dealloc,              /* tp_dealloc */
        0,                                             /* tp_print */
        0,                                             /* tp_getattr */
        0,                                             /* tp_setattr */
        0,                                             /* tp_reserved */
        (reprfunc) trinity_cell_tostring,              /* tp_repr */
        0,                                             /* tp_as_number */
        0,                                             /* tp_as_sequence */
        0,                                             /* tp_as_mapping */
        0,                                             /* tp_hash  */
        0,                                             /* tp_call */
        (reprfunc) trinity_cell_tostring,              /* tp_str */
        0,                                             /* tp_getattro */
        0,                                             /* tp_setattro */
        0,                                             /* tp_as_buffer */
        Py_TPFLAGS_DEFAULT,                            /* tp_flags */
        "Trinity ICell",                               /* tp_doc */
        0,                         /* tp_traverse */
        0,                         /* tp_clear */
        0,                         /* tp_richcompare */
        0,                         /* tp_weaklistoffset */
        0,                         /* tp_iter */
        0,                         /* tp_iternext */
        0,                         /* tp_methods */
        0,                         /* tp_members */
        0,                         /* tp_getset */
        0,                         /* tp_base */
        0,                         /* tp_dict */
        0,                         /* tp_descr_get */
        0,                         /* tp_descr_set */
        0,                         /* tp_dictoffset */
        (initproc)trinity_cell_init, /* tp_init */
        0,                           /* tp_alloc */
        0,                 /* tp_new */
    };

    static TRINITY_INTERFACES *pTrinity = NULL;

    PyObject*
    _Init(PyObject *self)
    {
        pTrinity = TRINITY_FFI_GET_INTERFACES();
        Py_RETURN_NONE;
    }
    
    PyObject*
    sync_registry(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    async_registry(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }


    PyObject*
    sync_send(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    async_send(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }


    PyObject*
    local_loadstorage(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    local_savestorage(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    local_resetstorage(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }


    PyObject*
    cloud_loadstorage(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cloud_savestorage(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cloud_resetstorage(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    local_loadcell(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cloud_loadcell(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }


    PyObject*
    local_savecell_1(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    local_savecell_2(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cloud_savecell(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }


    PyObject*
    local_removecell(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cloud_removecell(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }


    PyObject*
    newcell_1(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    newcell_2(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    newcell_3(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }


    PyObject*
    cell_tostring(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cell_getid(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cell_setid(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cell_hasfield(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cell_getfield(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cell_setfield(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cell_appendfield(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    PyObject*
    cell_removefield(PyObject *self, PyObject *args, PyObject *kwds)
    {
        return NULL;
    }

    // {{{cell object functions
    int64_t
    trinity_cell_init(TrinityCell_Object *self, PyObject *args, PyObject *kwds)
    {
        char *cellType = NULL, *cellContent = NULL;
        int64_t cellId = 0;
    
        static char *kwlist_1[] = {"cellType", "cellId", NULL};
        static char *kwlist_2[] = {"cellType", "cellContent", NULL};

        if (PyArg_ParseTupleAndKeywords(args, kwds, "is", kwlist_1, &cellType, &cellId))
        {
            return (TrinityErrorCode::E_SUCCESS == pTrinity->newcell_2(cellId, cellType, &(self->cell_handle)))
            ? 0 : -1;
        }

        if (PyArg_ParseTupleAndKeywords(args, kwds, "s|s", kwlist_2, &cellType, &cellContent))
        {
            if(!cellContent)
            {
                return (TrinityErrorCode::E_SUCCESS == pTrinity->newcell_1(cellType, &(self->cell_handle)))
                ? 0 : -1;
            }
            else
            {
                return (TrinityErrorCode::E_SUCCESS == pTrinity->newcell_3(cellType, cellContent, &(self->cell_handle)))
                ? 0 : -1;
            }
        }
    
        return -1;
    }

    void
    trinity_cell_dealloc(TrinityCell_Object* self)
    {
        pTrinity->cell_dispose(self->cell_handle);
        Py_TYPE(self)->tp_free((PyObject*)self);
    }

    PyObject* 
    trinity_cell_tostring(TrinityCell_Object* self)
    {
        char* str = pTrinity->cell_tostring(self->cell_handle);
        PyObject* obj = PyUnicode_FromString(str);
        free(str);
        return obj;
    }
    //}}}

    static PyMethodDef trinity_ffi_methods[] = {
        {"Init", (PyCFunction)_Init, METH_NOARGS, "Initialize the interfaces. Called upon Python runtime initialization."},
        {"sync_registry", (PyCFunction)sync_registry, METH_VARARGS, ""},
        {"async_registry", (PyCFunction)async_registry, METH_VARARGS, ""},
        {"sync_send", (PyCFunction)sync_send, METH_VARARGS, ""},
        {"async_send", (PyCFunction)async_send, METH_VARARGS, ""},
        {"local_loadstorage", (PyCFunction)local_loadstorage, METH_VARARGS, ""},
        {"local_savestorage", (PyCFunction)local_savestorage, METH_VARARGS, ""},
        {"local_resetstorage", (PyCFunction)local_resetstorage, METH_VARARGS, ""},
        {"cloud_loadstorage", (PyCFunction)cloud_loadstorage, METH_VARARGS, ""},
        {"cloud_savestorage", (PyCFunction)cloud_savestorage, METH_VARARGS, ""},
        {"cloud_resetstorage", (PyCFunction)cloud_resetstorage, METH_VARARGS, ""},
        {"local_loadcell", (PyCFunction)local_loadcell, METH_VARARGS, ""},
        {"cloud_loadcell", (PyCFunction)cloud_loadcell, METH_VARARGS, ""},
        {"local_savecell_1", (PyCFunction)local_savecell_1, METH_VARARGS, ""},
        {"local_savecell_2", (PyCFunction)local_savecell_2, METH_VARARGS, ""},
        {"cloud_savecell", (PyCFunction)cloud_savecell, METH_VARARGS, ""},
        {"local_removecell", (PyCFunction)local_removecell, METH_VARARGS, ""},
        {"cloud_removecell", (PyCFunction)cloud_removecell, METH_VARARGS, ""},
        {"cell_tostring", (PyCFunction)cell_tostring, METH_VARARGS, ""},
        {"cell_getid", (PyCFunction)cell_getid, METH_VARARGS, ""},
        {"cell_setid", (PyCFunction)cell_setid, METH_VARARGS},
        {"cell_hasfield", (PyCFunction)cell_hasfield, METH_VARARGS, ""},
        {"cell_getfield", (PyCFunction)cell_getfield, METH_VARARGS, ""},
        {"cell_setfield", (PyCFunction)cell_setfield, METH_VARARGS, ""},
        {"cell_appendfield", (PyCFunction)cell_appendfield, METH_VARARGS, ""},
        {"cell_removefield", (PyCFunction)cell_removefield, METH_VARARGS, ""},
        {NULL, NULL, 0, NULL}};

    static struct PyModuleDef trinity_ffi_module = {
        PyModuleDef_HEAD_INIT,
        "trinity",
        "Provides interoperatibility between Python and Trinity Graph Engine.",
        -1,
        trinity_ffi_methods};

    PyObject*
    PyInit_trinity()
    {
        PyObject *m;

        trinity_cellType.tp_new = PyType_GenericNew;
        if (PyType_Ready(&trinity_cellType) < 0)
            return NULL;
        m = PyModule_Create(&trinity_ffi_module);
        if (m == NULL)
            return NULL;

        Py_INCREF(&trinity_cellType);
        PyModule_AddObject(m, "cell", (PyObject *)&trinity_cellType);

        return m;
    }
}
