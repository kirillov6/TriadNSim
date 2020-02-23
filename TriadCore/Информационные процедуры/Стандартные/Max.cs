using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
{
    /// <summary>
    /// �������������� ������� ���������� ������������� �������� ���������
    /// </summary>
    public class IPMax: IProcedure
    {
        /// <summary>
        /// ������������ �������� ���������
        /// </summary>
        private Double max;
        private Boolean first;

        public static string Description = "���������� ������������� �������� ���������";
        
        /// <summary>
        /// �������� �������
        /// </summary>
        private Double Arg
        {
            get
            {   
                return ((Double)GetSpyVarValue(new CoreName("Arg")));
            }
            set
            {
                SetSpyVarValue(new CoreName("Arg"), value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public void RegisterSpyObjects(SpyObject arg)
        {
            RegisterSpyObject(arg, new CoreName("Arg"));
            RegisterSpyHandler(arg, DoHandling);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void DoInitialize()
        {
            this.max = 0;
            this.first = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectInfo"></param>
        /// <param name="systemTime"></param>
        protected override void DoHandling(SpyObject objectInfo, double systemTime)
        {
            if (first || Arg > max)
            {
                max = Arg;
                first = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Double DoProcessing()
        {
            return max;
        }
    }
}
