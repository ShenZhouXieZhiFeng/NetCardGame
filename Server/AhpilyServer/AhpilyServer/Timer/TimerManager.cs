using AhpilyServer.Concurrent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AhpilyServer.Timer
{
    /// <summary>
    /// 定时任务管理类
    /// </summary>
    public class TimerManager
    {
        private static TimerManager instance = null;
        /// <summary>
        /// 单例
        /// </summary>
        public static TimerManager Instance
        {
            get {
                //针对多线程
                lock (instance)
                {
                    if (instance == null)
                        instance = new TimerManager();
                    return instance;
                }
            }
        }

        /// <summary>
        /// 获取id
        /// </summary>
        private ConcurrentInt id;

        /// <summary>
        /// 定时器
        /// </summary>
        private System.Timers.Timer timer;

        /// <summary>
        /// 存储任务id和模型的字典
        /// </summary>
        private ConcurrentDictionary<int, TimerModel> idModelDict = new ConcurrentDictionary<int, TimerModel>();

        /// <summary>
        /// 将要移除的任务id列表
        /// </summary>
        private List<int> removeList = new List<int>();

        public TimerManager()
        {
            id = new ConcurrentInt(-1);
            timer = new System.Timers.Timer(10);
            timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// 每时间间隔触发一次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (removeList)
            {
                TimerModel tmpModel = null;
                foreach (var id in removeList)
                {
                    idModelDict.TryRemove(id, out tmpModel);
                }
                removeList.Clear();
            }
            foreach (var model in idModelDict.Values)
            {
                //如果时间到达，则执行该定时器的任务
                if (model.Time <= DateTime.Now.Ticks)
                {
                    model.Run();
                    removeList.Add(model.Id);
                }
            }
        }

        /// <summary>
        /// 添加定时任务 指定触发的时间
        /// </summary>
        public void AddTimerEvent(DateTime dateTime,TimerDelegate timerDelegate)
        {
            long delaytime = dateTime.Ticks - DateTime.Now.Ticks;
            if (delaytime <= 0)
                return;
            AddTimerEvent(delaytime, timerDelegate);
        }

        /// <summary>
        /// 添加定时任务 指定延迟的时间
        /// </summary>
        /// <param name="delayTime">延迟的时间</param>
        /// <param name="timeDelegate"></param>
        public void AddTimerEvent(long delayTime, TimerDelegate timerDelegate)
        {
            TimerModel model = new TimerModel(id.Add_Get(), DateTime.Now.Ticks + delayTime, timerDelegate);
            idModelDict.TryAdd(model.Id, model);
        }
    }
}
