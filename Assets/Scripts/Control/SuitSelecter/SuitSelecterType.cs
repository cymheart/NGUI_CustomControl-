using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlNS
{
    /// <summary>
    /// 花色
    /// </summary>
    public enum Suits
    {
        None = -1,
        /// <summary>
        /// 红方块
        /// </summary>
        Diamond = 0,
        /// <summary>
        /// 黑梅花
        /// </summary>
        Club = 1,
        /// <summary>
        /// 红心
        /// </summary>
        Heart = 2,
        /// <summary>
        /// 黑桃
        /// </summary>
        Spade = 3,
    }
}
