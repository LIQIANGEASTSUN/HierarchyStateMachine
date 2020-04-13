
namespace HSMTree
{
    public class HSMStateEntry : HSMState
    {
        //public static CustomIdentification _customIdentification = new CustomIdentification("Node/Entry", IDENTIFICATION.STATE_ENTRY, typeof(HSMStateEntry), NODE_TYPE.ENTRY);

        public HSMStateEntry() :base()
        {

        }

        public override void Init()
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            //UnityEngine.Debug.LogError("HSMStateEntry Enter:" + NodeId);
        }

        /// <summary>
        /// 执行节点抽象方法
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override int Execute(ref bool result)
        {
            int toStateId = -1;
            toStateId = base.Execute(ref result);
            return toStateId;
        }

        public override void Exit()
        {
            //UnityEngine.Debug.LogError("HSMStateEntry Exit:" + NodeId);
        }

    }
}