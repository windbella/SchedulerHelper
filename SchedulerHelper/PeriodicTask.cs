using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerHelper
{
    /// <summary>
    /// 제  목: PeriodicTask
    /// 작성자: dohong
    /// 작성일: 2017-10-13
    /// 설  명: 일정 시간 단위로 반복되는 작업
    /// 수정자: 수정자<탭>수정내용<탭>수정일자
    /// </summary>
    public class PeriodicTask
    {
        public event Action<object> Started; // 작업 시작 이벤트
        public event Action<object> DoTask; // 반복 작업 이벤트
        public event Action<object> Stopped; // 작업 종료 이벤트

        public bool IsBusy { get; protected set; } // 작업 실행 여부
        public TimeSpan Interval { get; set; } // 작업 간격
        public object Argument { get; set; } // 작업에 사용될 파라메터 (필요할 시 사용)

        public PeriodicTask()
        {
            IsBusy = false;
            Interval = new TimeSpan(0, 0, 10);
        }

        // 작업 시작
        public void Start()
        {
            if (IsBusy)
            {
                return;
            }
            IsBusy = true;
            if (Started != null)
            {
                Started(Argument);
            }
            cancellation = new CancellationTokenSource();
            task = LoopDoTask();
        }

        // 작업 중단
        public void Stop()
        {
            if (!IsBusy)
            {
                return;
            }
            cancellation.Cancel();
            IsBusy = false;
            if (Stopped != null)
            {
                Stopped(Argument);
            }
        }

        // 실제 작업 테스크
        public Task Task
        {
            get { return task; }
        }

        #region 내부함수
        private Task task; // 작업 개체
        protected CancellationTokenSource cancellation; // 작업 중단 토큰

        // 반복작업 함수
        protected virtual async Task LoopDoTask()
        {
            while (!cancellation.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(Convert.ToInt32(Interval.TotalMilliseconds), cancellation.Token);
                    ExcuteDoTask();
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        // 작업 실행
        public void ExcuteDoTask()
        {
            if (DoTask != null)
            {
                DoTask(Argument);
            }
        } 
        #endregion
    }
}
