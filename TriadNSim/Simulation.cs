using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DrawingPanel;
using TriadPad;
using TriadCompiler;
using System.Diagnostics;
using System.Reflection;
using TriadNSim.Forms;

namespace TriadNSim
{
    public class IPResult
    {
        public NetworkObject SpyObject;
        public ConnectedIP ConnectedIP;
        public object Result;

        public IPResult(NetworkObject spyObject, ConnectedIP ip)
        {
            this.SpyObject = spyObject;
            this.ConnectedIP = ip;
        }

        public string Name
        {
            get
            {
                string sRes = ConnectedIP.IP.Name;
                if (ConnectedIP.Params.Count > 0)
                {
                    sRes += "(";
                    for (int i = 0; i < ConnectedIP.Params.Count; i++)
                    {
                        if (i > 0)
                            sRes += ",";
                        sRes += ConnectedIP.Params[i];
                    }
                    sRes += ")";
                }
                return sRes;
            }
        }

        public string Description
        {
            get
            {
                if (ConnectedIP.Description.Length == 0)
                    return ConnectedIP.IP.Description;
                return ConnectedIP.Description;
            }
        }
    }

    public class Simulation
    {
        String fileName = "New.txt";
        String CompiledFileName = ".\\New.dll";
        public List<IPResult> IPResults = new List<IPResult>();

        bool NeedServerRoutine = false;
        bool NeedClientRoutine = false;
        
        public DrawingPanel.DrawingPanel drawingPanel;
        private frmMain mainForm;
        private static Simulation _inst;

        public static Simulation Instance
        {
            get
            {
                if (_inst == null)
                    _inst = new Simulation(frmMain.Instance, frmMain.Instance.Panel);
                return _inst;
            }
        }
        private Simulation(frmMain mainForm, DrawingPanel.DrawingPanel panel)
        {
            this.mainForm = mainForm;
            drawingPanel = panel;
        }

        public void Begin(SimulationInfo simInfo)
        {
            IPResults.Clear();
            NSDesign design = new NSDesign(simInfo);
            //Вызываем метод, строящий модель, и производящий моделирование
            object graph = design.Build();

            //string sXml = TriadCore.Logger.Instance.XML;
            frmResult frmRes = new frmResult();
            frmRes.Fill();
            frmRes.ShowDialog(mainForm);
        }

        public void Start(bool bSimulate)
        {
            if (CreateModel(bSimulate) && CompileModel())
                Run();
        }

        public bool CreateModel(bool bSimulate)
        {
            FileStream stream = null;
            StreamWriter writer = null;
            Encoding enc = Encoding.GetEncoding(1251);
            try
            {
                stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(stream, enc);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            writer.WriteLine("design D");
            #region WriteModel
            writer.WriteLine("model M");
            #region WriteStructureToFile
            ArrayList all = drawingPanel.Shapes;
            List<Link> links = new List<Link>();
            List<NetworkObject> shapes = new List<NetworkObject>();
            List<NetworkObject> SpyObjects = new List<NetworkObject>();
            foreach (BaseObject obj in all)
            {
                if (obj is Link)
                    links.Add(obj as Link);
                else if (obj is NetworkObject)
                {
                    NetworkObject NetObj = obj as NetworkObject;
                    shapes.Add(NetObj);
                    if (NetObj.ConnectedIPs.Count > 0)
                        SpyObjects.Add(NetObj);
                }
            }
            String text = "";
            writer.WriteLine("structure Network def");
            if (shapes.Count > 0)
            {
                NetworkObject shape = shapes[0];

                if (shape is Server)
                    NeedServerRoutine = true;
                else if (shape is Client)
                    NeedClientRoutine = true;
                text = "Network := node " + shape.Name;
                if (shape.Routine.Poluses.Count > 0)
                    text += "<" + shape.Routine.Poluses[0].Name;
                for (int i = 1; i < shape.Routine.Poluses.Count; i++)
                    text += "," + shape.Routine.Poluses[i].Name;
                if (shape.Routine.Poluses.Count > 0)
                    text += ">";
                writer.Write(text);
                for (int i = 1; i < shapes.Count; i++)
                {
                    shape = shapes[i];
                    text = " + node " + shape.Name;
                    if (shape.Routine.Poluses.Count > 0)
                        text += "<" + shape.Routine.Poluses[0].Name;
                    for (int j = 1; j < shape.Routine.Poluses.Count; j++)
                        text += "," + shape.Routine.Poluses[j].Name;
                    if (shape.Routine.Poluses.Count > 0)
                        text += ">";
                    writer.Write(text);
                    if (shape is Server)
                        NeedServerRoutine = true;
                    else if (shape is Client)
                        NeedClientRoutine = true;
                }
                text = ";\n";
                writer.Write(text);
                if (links.Count > 0)
                {
                    text = "Network := Network";
                    writer.Write(text);
                    for (int i = 0; i < links.Count; i++)
                    {
                        Link link = links[i];
                        NetworkObject ObjFrom = link.FromCP.Owner as NetworkObject;
                        NetworkObject ObjTo = link.ToCP.Owner as NetworkObject;
                        text = " + edge ( " + ObjFrom.Name + "." + link.PolusFrom + " -- " + ObjTo.Name + "." + link.PolusTo + " )";
                        writer.Write(text);
                    }
                    text = ";\n";
                    writer.Write(text);
                }
            }
            text = "endstr\n\n";
            writer.Write(text);
            #endregion
            #region WriteRoutinesToFile
            foreach (NetworkObject obj in shapes)
            {
                if (obj is UserObject && obj.Routine.Text.Trim().Length > 0)
                {
                    writer.Write(obj.Routine.Text);
                    writer.WriteLine();
                }
            }
            if (NeedServerRoutine)
            {
                writer.WriteLine("include routine RServer [ integer максДлина; real deltaT ] from \"Routines.dll\"");
            }
            if (NeedClientRoutine)
            {
                writer.WriteLine("include routine RClient [ real deltaT ] from \"Routines.dll\"");
                writer.WriteLine();
            }
            #endregion
            writer.WriteLine("def");
            #region WriteBodyModel
            text = "structure topology;\n";
            text += "let Network() be topology;\n";
            text += "M := topology;\n";
            writer.Write(text);

            for (int i = 0; i < shapes.Count; i++)
            {
                NetworkObject shape = shapes[i];
                if (shape is UserObject && shape.Routine.Text.Trim().Length == 0)
                    continue;
                //экземпляр рутины
                string nameRoutine = "r" + shape.Name;
                text = "routine " + nameRoutine + ";\n";
                text += "let ";
                if (shape is Client)
                {
                    string s = "RClient(" + (shape as Client).DeltaT.ToString() + ")";
                    text += s;
                }
                else if (shape is Server)
                {
                    Server oServer = shape as Server;
                    string s = "RServer(" + oServer.MaxQueueLength.ToString() + "," + oServer.DeltaT.ToString() + ")";
                    text += s;
                }
                else
                {
                    text += shape.Routine.Name + "()";
                }
                text += " be " + nameRoutine + ";\n";
                writer.Write(text);
                text = "put " + nameRoutine + " on M." + shape.Name;
                if (shape.Routine.Poluses.Count > 0)
                {
                    string name = shape.Routine.Poluses[0].Name;
                    text += "<" + name + "=" + name;
                }
                for (int j = 1; j < shape.Routine.Poluses.Count; j++)
                {
                    string name = shape.Routine.Poluses[j].Name;
                    text += "," + name + "=" + name;
                }
                if (shape.Routine.Poluses.Count > 0)
                    text += ">";
                text += ";\n";
                writer.Write(text);
            }
            #endregion
            writer.WriteLine("endmod");
            #endregion
            WriteSimCondition(writer, SpyObjects);
            #region WriteBodyDesign
            writer.WriteLine("def");
            writer.WriteLine("model m;");
            writer.WriteLine("let M() be m;");
            text = "simulate m on IC[" + mainForm.GetEndModelTime().ToString() + "](";
            //start write inf proc params
            int nParamCount = 0;
            foreach (NetworkObject NetObj in SpyObjects)
            {
                int nCount = NetObj.ConnectedIPs.Count;
                for (int i = 0; i < nCount; i++)
                {
                    if (nParamCount != 0)
                        text += ",";
                    nParamCount++;
                    ConnectedIP ConnIP = NetObj.ConnectedIPs[i];
                    foreach (string sParamName in ConnIP.Params)
                        text += NetObj.Name + "." + sParamName;
                }
            }
            text += ");";
            //end write inf proc params
            writer.WriteLine(text);
            writer.WriteLine("enddes");
            #endregion
            writer.Close();
            return true;
        }

        private void WriteSimCondition(StreamWriter writer, List<NetworkObject> SpyObjects)
        {
            writer.Write("simcondition IC");
            //start write inf proc
            Dictionary<string, InfProcedure> InfProcedures = new Dictionary<string, InfProcedure>();
            int nParamCount = 0;
            foreach (NetworkObject NetObj in SpyObjects)
            {
                foreach (ConnectedIP ConnIP in NetObj.ConnectedIPs)
                {
                    int nCount = ConnIP.IP.Params.Count;
                    for (int i = 0; i < nCount; i++)
                    {
                        writer.Write(nParamCount == 0 ? "(" : ";");
                        nParamCount++;
                        IPParam param = ConnIP.IP.Params[i];
                        string sParam = string.Empty;
                        if (!param.IsPolus && !param.IsEvent)
                            sParam += "in ";
                        sParam += param.TypeName + " var" + nParamCount.ToString();
                        writer.Write(sParam);
                    }
                    if (!ConnIP.IP.IsStandart && !InfProcedures.ContainsKey(ConnIP.IP.Name))
                        InfProcedures[ConnIP.IP.Name] = ConnIP.IP;
                }
            }
            if (nParamCount > 0)
                writer.Write(")");
            writer.Write("[real terminateTime]");
            writer.WriteLine();
            foreach (InfProcedure ip in InfProcedures.Values)
                writer.WriteLine(ip.Code);
            //end write inf proc
            writer.WriteLine("def");
            nParamCount = 0;
            Dictionary<ConnectedIP, string> IPRes = new Dictionary<ConnectedIP, string>();
            foreach (NetworkObject NetObj in SpyObjects)
            {
                foreach (ConnectedIP ConnIP in NetObj.ConnectedIPs)
                {
                    string sIPRes = string.Empty;
                    if (ConnIP.IP.ReturnCode != TriadCompiler.TypeCode.UndefinedType)
                    {
                        sIPRes = "IPRes" + (IPRes.Count + 1).ToString();
                        writer.WriteLine(ConnIP.IP.ReturnCode.ToString() + " " + sIPRes + ";");
                        IPRes[ConnIP] = sIPRes;
                    }
                    writer.Write(ConnIP.IP.Name + "(");
                    int nCount = ConnIP.IP.Params.Count;
                    for (int i = 0; i < nCount; i++)
                    {
                        if (i != 0)
                            writer.Write(",");
                        nParamCount++;
                        writer.Write("var" + nParamCount.ToString());
                    }
                    writer.Write(")");
                    if (sIPRes.Length != 0)
                        writer.Write(" : " + sIPRes);
                    writer.Write(";");
                    writer.WriteLine();
                }
            }
            writer.WriteLine("if SystemTime >= terminateTime then");
            //ip res
            nParamCount = 0;
            foreach (NetworkObject NetObj in SpyObjects)
            {
                foreach (ConnectedIP ConnIP in NetObj.ConnectedIPs)
                {
                    if (IPRes.ContainsKey(ConnIP))
                    {
                        nParamCount++;
                        writer.Write("Print \"" + NetObj.Name + " - " + ConnIP.IP.Name + " : \" + ");
                        if (ConnIP.IP.ReturnCode != TriadCompiler.TypeCode.String)
                            writer.Write(ConnIP.IP.ReturnCode.ToString() + "ToStr(" + IPRes[ConnIP] + ");");
                        else
                            writer.Write("var" + IPRes[ConnIP] + ";");
                        writer.WriteLine();
                    }
                }
            }
            //ip res
            writer.WriteLine("eor");
            writer.WriteLine("endif");
            writer.WriteLine("endcond");
        }

        public bool CompileModel()
        {
            Encoding enc = Encoding.GetEncoding(1251);
            FileStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                reader = new StreamReader(stream, enc);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            stream.Seek(0, SeekOrigin.Begin);
            RichTextBox text = new RichTextBox();
            text.Text = reader.ReadToEnd();
            reader.Close();
            RichTextBox listing = new RichTextBox();
            InputRichEdit input = new InputRichEdit(text);
            OutputRichEdit output = new OutputRichEdit(listing);
            IOErrorListener io = new IOErrorListener(input, output);
            CompilerFacade.CompileDesignToDll(io, CompiledFileName);
            ErrorDescription[] errs = io.getRegisteredErrors();
            if (errs.GetLength(0) > 0)
            {
                StreamWriter oWriter = 
                    new StreamWriter(new FileStream("Log.txt", FileMode.Create, FileAccess.Write), Encoding.GetEncoding(1251));
                foreach (ErrorDescription err in errs)
                {
                    oWriter.Write("(" + err.lineNumber.ToString() + "," + err.chNumber.ToString() + ") ");
                    oWriter.WriteLine(err.ToString());
                }
                oWriter.Close();
                Util.ShowErrorBox("Произошла ошибка, обратитесь к разработчику");
                return false;
            }
            return true;
        }

        public void Run()
        {
            try
            {
                /*mainForm.Cursor = Cursors.WaitCursor;
                Process p = new Process();
                p.StartInfo.FileName = "TriadRunner.exe";
                p.StartInfo.Arguments = "New.dll D -1 res.txt";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
                p.WaitForExit();
                frmResult frmRes = new frmResult();
                mainForm.Cursor = Cursors.Default;
                frmRes.ShowDialog(mainForm);*/
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
