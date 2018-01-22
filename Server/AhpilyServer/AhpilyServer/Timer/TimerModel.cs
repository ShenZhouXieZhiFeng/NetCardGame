using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer.Timer
{
    /// <summary>
    /// 当定时器达到时间时触发
    /// </summary>
    public delegate void TimerDelegate();

    /// <summary>
    /// 定时器任务数据模型
    /// </summary>
    public class TimerModel
    {

        /// <summary>
        /// 唯一标识
        /// </summary>
        public int Id;

        /// <summary>
        /// 任务执行时间
        /// </summary>
        public long Time;

        private TimerDelegate timerDelegate;

        public TimerModel(int id, long time, TimerDelegate timerDelegate)
        {
            Id = id;
            Time = time;
            this.timerDelegate = timerDelegate;
        }

        /// <summary>
        /// 触发任务
        /// </summary>
        public void Run()
        {
            timerDelegate();
        }

    }

}
