namespace HSMTree
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum NODE_TYPE
    {
        /// <summary>
        /// State
        /// </summary>
        [EnumAttirbute("State")]
        STATE = 0,

        /// <summary>
        /// Sub-Machine
        /// </summary>
        [EnumAttirbute("Sub-Machine")]
        SUB_STATE_MACHINE = 1,

        /// <summary>
        /// Entry
        /// </summary>
        [EnumAttirbute("Entry")]
        ENTRY = 2,

        /// <summary>
        /// EXIT
        /// </summary>
        [EnumAttirbute("Exit")]
        EXIT = 3,
    }
}