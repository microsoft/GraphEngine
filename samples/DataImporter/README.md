# Graph Engine DataImporter

DataImporter is a cross-platform module of Microsoft [Graph Engine](https://www.graphengine.io/) for [data importing](https://www.graphengine.io/docs/manual/DataImport.html). It supports to import data files in types of CSV, TSV, JSON and RDF, saving input data to Graph Engine storages.

## Usage
To use DataImporter, you need to [build Graph Engine](https://github.com/Microsoft/GraphEngine) first.
The input files include [TSL file](https://www.graphengine.io/docs/manual/TSL/tsl-basics.html) and data file,
and the output is Graph Engine data storage.

### TSL File
The content of the TSL file must follow the syntax of TSL. The fields defined in TSL file need to correspond to the fields in data files. 

### Data File
Only data files with .json, .csv, .tsv and .ntriples suffix are recognized.
The file name of a data file will be used as the type for deserialization (except for RDF files).
The type must be defined as a Graph Engine cell in the TSL.

### Command Line Option

|Option | Description|
|:-----|:------------|
|-t, --tsl|Specifies the TSL file for data importing.|
|-d, --dir|Import all .json and .txt files from directory.|
|-o, --output|Specifies data import output directory for importing tasks, and specifies the output TSL file name for TSL generation tasks.|
|-g, --generate_tsl|Generates TSL.|
|-s, --sorted|Specifies that the data is already sorted/grouped by entities.|
|--delimiter|Specifies the delimiter of CSV or TSV file.|
|-f, --fileFormat|Specifies the file format.|
|--notrim|Specifies that the data fields in CSV/TSV files are not trimmed.|
|-a, --tsl_assembly|Specifies the TSL assembly for data importing.|
|--help|Display this help screen.|

  
### In Windows
Open the `GraphEngine.DataImporter.sln` with Visual Studio 2017/2015, build the project and generate the GraphEngine.DataImporter.exe (the exe can't move to other folders).  

```
GraphEngine.DataImporter.exe [-t tsl] [-d directory] [-o output_dir] [--delimiter delimiter] [-f file_format] [--notrim] [-a tsl_assembly|-g] [explicit files]
```

### In Linux
In `GraphEngine.DataImporter` folder:   

```
dotnet restore GraphEngine.DataImporter.Clr.csproj  
dotnet run -p GraphEngine.DataImporter.Clr.csproj [-t tsl] [-d directory] [-o output_dir] [--delimiter delimiter] [-f file_format] [--notrim] [-a tsl_assembly|-g] [explicit files]
```




