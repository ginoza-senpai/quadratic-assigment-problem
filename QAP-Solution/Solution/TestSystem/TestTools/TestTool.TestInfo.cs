﻿using System;
using System.IO;

namespace TestSystem
{
    public class CTestInfo
    {
        public struct SExam
        {
            public SExam(long val = 0, bool ok = false){ bInit = ok; value = val; }
            public bool IsInit() => bInit;
            public long Value() => value;
            long value;
            bool bInit;
        }

        public string pathProblem { get; }
        SExam m_val;

        public CTestInfo(string problem, string resultPath = "")
        {
            pathProblem = problem;
            if(resultPath == "")
                m_val = new SExam();
            else
            {
                StreamReader file = new StreamReader(resultPath);
                string str = file.ReadToEnd();
                file.Close();
                str.Trim(' ');
                str = str.Replace("\r\n", "\n");
                string[] strSplitN = str.Split('\n');
                string[] strSplitNSpace = strSplitN[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                m_val = new SExam(Convert.ToInt64(strSplitNSpace[1]), true);
            }
        }
        public bool Exam(ref long obj)
        {
            obj = m_val.Value();
            return m_val.IsInit();
        }
        public string Name() => pathProblem.Substring(pathProblem.LastIndexOf("\\") + 1, pathProblem.LastIndexOf('.') - pathProblem.LastIndexOf("\\") - 1);
        public void GenerateResultFile(string path, int size, long result, string perm)
        {
            if(!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            string resFilePath = path + Name() + ".bin";
            if(!System.IO.File.Exists(resFilePath))
                System.IO.File.Create(resFilePath).Close();
            StreamWriter wr = new StreamWriter(resFilePath);
            wr.Write($"{size} {result}\n{perm.Substring(0, perm.IndexOf(':'))}");
            wr.Close();
        }
    }
}