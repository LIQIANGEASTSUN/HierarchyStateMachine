namespace HSMTree
{
    /// <summary>
    /// 行为树节点类型
    /// </summary>
    public enum NODE_TYPE
    {
        /// <summary>
        /// 状态节点
        /// </summary>
        [EnumAttirbute("状态节点")]
        STATE = 0,

        /// <summary>
        /// 子状态机
        /// </summary>
        [EnumAttirbute("子状态机")]
        SUB_STATE_MACHINE = 1,
    }
}