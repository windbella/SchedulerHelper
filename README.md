### 반복 작업 라이브러리
윈도우 서비스 등을 이용하여 데몬을 만들 때 유용하게 사용할 수 있는 라이브러리이다.
주기적 실행, 특정 시간 (시간, 분) 실행을 지원한다.

```
PeriodicTask task = new PeriodicTask();
task.Interval = new TimeSpan(0, 0, 10);
task.Started += (obj) =>
{
    Debug.WriteLine("시작");
};
task.Stopped += (obj) =>
{
    Debug.WriteLine("중단");
};
task.DoTask += (obj) =>
{
    Debug.WriteLine("작업 중");
};
task.Start();
```

기본적인 주기적 반복 작업 예제이다. Interval(반복 주기)를 설정하고 이벤트를 등록한 후 Start 메소드를 실행하면 작동한다.

```
WeeklyTask task = new WeeklyTask();
task.Interval = new TimeSpan(0, 0, 10);
task.AddDaily(15, 30);
task.Schedule.Add(WeeklyTask.NewDateTime(DayOfWeek.Monday, 12, 30));
task.DoTask += Process;
task.Start();
```
```
public void Process(object obj)
{
    Debug.WriteLine("작업 중");
}
```
위와 같이 작성하게 되면 매일 15시 30분에 작동하고 추가로 월요일 12시 30분에도 작동하게 된다.
WeeklyTask에서의 Interval은 시간 검사 주기를 의미한다. 주기가 10초라면 10초 정도의 오차가 날 수 있다.
(15시 30분 0초 ~ 15시 30분 10초 사이에 작동)
