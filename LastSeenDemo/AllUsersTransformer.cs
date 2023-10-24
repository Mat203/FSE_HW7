namespace LastSeenDemo;

public class GlobalMetrics
    {
        public int DailyAverage { get; set; }
    }

public class AllUsersTransformer
{
    private readonly IUserTransformer _transformer;
    private readonly IOnlineDetector _detector;
    public AllUsersTransformer(IUserTransformer transformer, IOnlineDetector detector) 
    {
        _transformer = transformer;
        _detector = detector; 
    }

    public void Transform(IEnumerable<User> allUsers, List<Guid> onlineUsers, Dictionary<Guid, List<UserTimeSpan>> result)
    {
        foreach (var user in allUsers)
        {
            if (!result.TryGetValue(user.UserId, out var userTimeSpans))
            {
                userTimeSpans = new List<UserTimeSpan>();
                result.Add(user.UserId, userTimeSpans);
            }
            var wasOnline = onlineUsers.Contains(user.UserId);
            _transformer.TransformSingleUser(user, wasOnline, userTimeSpans);

            if (!wasOnline && user.IsOnline)
            {
                onlineUsers.Add(user.UserId);
            }
            else if (!user.IsOnline)
            {
                onlineUsers.Remove(user.UserId);
            }
        }
    }

    public GlobalMetrics CalculateGlobalMetrics(Dictionary<Guid, List<UserTimeSpan>> userTimeSpans)
    {
        var globalMetrics = new GlobalMetrics
        {
            DailyAverage = _detector.CalculateGlobalDailyAverageForAllUsers(userTimeSpans), 
        };

        return globalMetrics;
    }
}