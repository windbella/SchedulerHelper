using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerHelper
{
    /// <summary>
    /// 제  목: WeeklyTask
    /// 작성자: dohong
    /// 작성일: 2017-10-13
    /// 설  명: 매주 [요일, 시간, 분]에 반복되는 작업
    /// 수정자: 수정자<탭>수정내용<탭>수정일자
    /// </summary>
    public class WeeklyTask : PeriodicTask
    {
        public HashSet<DateTime> Schedule { get; set; } // 작업 실행 시기 (NewDateTime으로 DateTime을 생성하여 Add)

        public WeeklyTask()
        {
            IsBusy = false;
            Interval = new TimeSpan(0, 0, 10);
            Schedule = new HashSet<DateTime>();
        }

        // [요일,시간,분]으로 DateTime 생성
        public static DateTime NewDateTime(DayOfWeek day, int hour, int minute)
        {
            DateTime result = new DateTime(2017, 1, 1, hour, minute, 0);
            result = result.AddDays((int)day);
            return result;
        }

        public void AddDaily(int hour, int minute)
        {
            Schedule.Add(WeeklyTask.NewDateTime(DayOfWeek.Friday, hour, minute));
            Schedule.Add(WeeklyTask.NewDateTime(DayOfWeek.Monday, hour, minute));
            Schedule.Add(WeeklyTask.NewDateTime(DayOfWeek.Saturday, hour, minute));
            Schedule.Add(WeeklyTask.NewDateTime(DayOfWeek.Sunday, hour, minute));
            Schedule.Add(WeeklyTask.NewDateTime(DayOfWeek.Thursday, hour, minute));
            Schedule.Add(WeeklyTask.NewDateTime(DayOfWeek.Tuesday, hour, minute));
            Schedule.Add(WeeklyTask.NewDateTime(DayOfWeek.Wednesday, hour, minute));
        }

        #region 내부함수
        private HashSet<DateTime> log = new HashSet<DateTime>(); // 실행된 시간대 저장

        // 반복작업 함수
        protected override async Task LoopDoTask()
        {
            while (!cancellation.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(Convert.ToInt32(Interval.TotalMilliseconds), cancellation.Token);

                    DateTime now = GetNow();
                    if (IsAction(now))
                    {
                        CleanUpLog();
                        log.Add(now);
                        ExcuteDoTask();
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        // 초를 제외한 현재시간
        private DateTime GetNow()
        {
            DateTime now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            return now;
        }

        // 일주일이 지난 로그 삭제
        private void CleanUpLog()
        {
            foreach (DateTime item in log.ToList())
            {
                if (item.CompareTo(DateTime.Now.AddDays(-7)) < 0)
                {
                    log.Remove(item);
                }
            }
        }

        // 작업 실행 여부 판단
        private bool IsAction(DateTime now)
        {
            bool isAction = false;
            foreach (DateTime item in Schedule)
            {
                if (now.DayOfWeek == item.DayOfWeek &&
                    now.Hour == item.Hour &&
                    now.Minute == item.Minute)
                {
                    if (!log.Contains(now))
                    {
                        isAction = true;
                        break;
                    }
                }
            }
            return isAction;
        } 
        #endregion
    }
}
