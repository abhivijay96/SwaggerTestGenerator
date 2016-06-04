using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace HK_internship
{
    class Generate
    {
        static int Gcount = 0;
        
        static Dictionary<string, List<SwaggerOperation>> operations = new Dictionary<string, List<SwaggerOperation>>();

        public static string generate()
        {
            try
            {
                return generateCodeFile();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error while generating Code : " + e);
                return null;
            }
        }

        public static void Initializer()
        {
            try
            {
                initializeParams();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error fetching or parsin the json file");
            }
        }

        private static string generateCodeFile()
        {
            string line;
            
            StreamReader file = null;
            try
            {
                file = new StreamReader(@"Generates.json");
            }
            catch(IOException)
            {
                return "No Records found";
            }
            
            string code = "";
            
            code += "//CODE GENERATED TO TEST THE SWAGGER API\n";
            
            code += "using System;\nusing Xunit;\nnamespace Testing\n{\n\tpublic class ApiTest\n\t{\n";
            
            
            code += "\t\t\tvar api = new APIV1(new Uri(\"http://localhost:5000/\"));\n\n";
            while ((line = file.ReadLine()) != null)
            {
                Gcount++;
                string temp = "";
                temp += map(Proxy.decode(line));
                code += "\t\t\t" + temp+"\n";
            }
            code += "\t}\n}";
           // file1.WriteLine("\t\t}\n\t}\n}");
            file.Close();
            //file1.Close();
            return code;
        }


        private static void initializeParams()
        {

            string swag = Proxy.swagger;
            JObject json = JObject.Parse(swag);
            JObject json1 = JObject.Parse(json["paths"].ToString());
            foreach (var x in json1.Children())
            {
                json = JObject.Parse(json1[x.Path.ToString()].ToString());
                List<SwaggerOperation> ops = new List<SwaggerOperation>();
                operations.Add(x.Path.ToString(), ops);
                foreach (var y in json.Children())
                {
                    JObject t = JObject.Parse(json[y.Path.ToString()].ToString());
                    SwaggerOperation temp = new SwaggerOperation(x.Path, y.Path.ToUpper(), t["operationId"].ToString());
                    if (t["responses"] != null)
                    {
                        JObject res = JObject.Parse(t["responses"].ToString());
                        res = JObject.Parse(res[res.First.Path].ToString());
                        if (res["schema"] != null)
                        {
                            temp.resDataType = res["schema"]["type"].ToString();
                            if (temp.resDataType.Equals("array"))
                                temp.resArrayType = res["schema"]["items"]["type"].ToString();
                        }
                    }
                    JArray a;
                    if (t["parameters"] != null)
                    {
                        a = JArray.Parse(t["parameters"].ToString());
                        foreach (var param in a.Children())
                        {
                            //Parameters(string name,string In,string type,string itype,string cf,bool required)
                            string name = param["name"].ToString();
                            string In = param["in"].ToString();
                            string type;
                            if (!In.Equals("body"))
                                type = param["type"].ToString();
                            else
                                type = param["schema"]["type"].ToString();
                            string item = null;
                            if (param["items"] != null)
                                item = param["item"]["type"].ToString();
                            string collectionFormat = "csv";
                            if (param["collectionFormat"] != null)
                                collectionFormat = param["collectionFormat"].ToString();
                            bool req = true;
                            if (param["required"] != null)
                                req = (bool)param["required"];
                            temp.addParam(new Parameters(name, In, type, item, collectionFormat, req));
                        }
                    }
                    //add operation to the dictionary
                    ops.Add(temp);
                }
            }
        }

        private static string map(Record r)
        {

            //replace all path parameters with regular expression 
            foreach (var x in operations.Keys)
            {
                List<SwaggerOperation> ops = operations[x];
                Regex rgx = new Regex("{.+}");
                string result = rgx.Replace(x, "[A-Za-z0-9_,]+");

                string text = r.req.path;
                result = @"^" + result + "$";
                //Console.WriteLine(result);
                // Instantiate the regular expression object.
                Regex reg = new Regex(result.ToLower(), RegexOptions.IgnoreCase);

                // Match the regular expression pattern against a text string.
                Match m = reg.Match(text.ToLower());
                if (m.Success)
                {
                    foreach (var y in ops)
                    {

                        if (y.method.Equals(r.req.method))
                            return getCode(y, r);
                    }
                }
            }
            return null;
        }

        private static string getCode(SwaggerOperation op, Record r)
        {
            string code = "";
            List<string> arrays = new List<string>();
            int count = 0, k = op.parameters.Count;
            //check if parameters is empty, if yes  return only the method name 
            if (op.parameters.Count == 0)
            {
                if (op.method.Equals("GET"))
                {
                    
                    string func = getFunc(op, r, arrays, code);
                    return func;
                }
                return "\\api." + op.operationId + "();";
            }

            foreach (Parameters p in op.parameters)
            {
                string value;
                string[] values;
                if (!p.type.Equals("array"))
                {
                    value = getVal(p, r, op);
                    if (p.type.Equals("integer"))
                    {
                        int n;
                        bool isNumeric = int.TryParse(value, out n);
                        if (!isNumeric)
                            value = "0";
                    }
                    if (p.type.Equals("string"))
                    {
                        value = "\"" + value + "\"";
                    }
                    if (count != k - 1)
                        code += value + ",";
                    else
                        code += value;
                }
                else
                {
                    values = getVal(p, r, op).Split(',');
                    string temp = getStringArr(values, arrays.Count, p.itype);
                    arrays.Add(temp);
                    if (count != k - 1)
                        code += "arr" + (arrays.Count - 1) + ",";
                    else
                        code += "arr" + (arrays.Count - 1);
                }
                count++;
            }

            if (r.req.method.Equals("GET"))
            {
                return getFunc(op, r, arrays, code);
            }

            return "//api." + op.operationId + "(" + code + ");";
        }

        private static string getFunc(SwaggerOperation op, Record r, List<string> arrays, string code)
        {
            string temp = "[FACT]\n";
            temp += "\t\t\tpublic void GetValuesTest" + Gcount + "() {\n";
            foreach (string s in arrays)
            {
                temp += "\t\t\t\t" + s;
            }
            
            temp += "\t\t\t\tvar output = " + "api." + op.operationId + "(" + code + ");\n";
            temp += "\t\t\t\tvar actual = ";
            if (op.resDataType.Equals("array"))
            {
                string[] val = r.res.body.Substring(1, r.res.body.Length - 2).Split(',');
                temp += "new []{ ";
                int len = val.Length - 1;
                int lcount = 0;
                foreach (var x in val)
                {
                    if (lcount < len)
                        temp += x + ",";
                    else
                        temp += x + "};\n";
                    lcount++;
                }
            }
            else
            {
                if (op.resDataType.Equals("string"))
                    temp += "\"" + r.res.body + "\";\n";
                else
                    temp += r.res.body + ";\n";
            }
            temp += "\t\t\t\tAssert.Equal(actual , output);\n\t\t\t}\n";
            return temp;
        }

        private static string getStringArr(string[] input, int count, string itype)
        {
            string code = "var arr" + count + " = new[] {";
            int k = input.Length;
            for (int i = 0; i < input.Length; i++)
            {
                var value = input[i];
                if (itype.Equals("string"))
                    value = "\"" + value + "\"";
                if (i != k - 1)
                    code += value + " , ";
                else
                    code += value + "};\n";
            }
            return code;
        }

        private static string getVal(Parameters p, Record r, SwaggerOperation op)
        {
            if (p.In == null)
                return "";
            if (p.In.Equals("path"))
            {
                //Console.WriteLine("OP path " + op.path);
                string x = HttpUtility.UrlDecode(r.req.path);
                string[] temp = op.path.Split('/');
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i].Equals("{" + p.name + "}"))
                        return (x.Split('/')[i]);
                }
                //Console.WriteLine("index : " + )
                return null;
            }
            else if (p.In.Equals("query"))
            {
                string q = HttpUtility.UrlDecode(r.req.path).Split('?')[1];
                string[] values = q.Split('&');
                foreach (string s in values)
                {
                    if (s.Split('=')[0].Equals(p.name))
                        return (s.Split('=')[1]);
                }
                return null;
            }
            else if (p.In.Equals("body"))
            {
                return r.req.body;
            }
            else
            {
                //case header
                return r.req.Headers[p.name];
            }
        }
    }
}
