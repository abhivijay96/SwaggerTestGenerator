using System;
using System.Collections.Generic;


namespace HK_internship
{
    public class Parameters
    {
        public string name, In, description, type, itype, collectionFormat;
        bool required;
        public Parameters(string name, string In, string type, string itype, string cf, bool required)
        {
            this.name = name;
            this.In = In;
            this.type = type;
            this.itype = itype;
            this.collectionFormat = cf;
            this.required = required;
        }
    }

    public class SwaggerOperation
    {
        public string path;
        public string method;
        public List<Parameters> parameters;
        public string operationId;
        public string resDataType;
        public string resArrayType;

        public SwaggerOperation(string path, string method, string operationId)
        {
            this.path = path;
            this.method = method;
            this.operationId = operationId;
            parameters = new List<Parameters>();
        }

        public void addParam(Parameters p)
        {
            parameters.Add(p);
        }
    }

}