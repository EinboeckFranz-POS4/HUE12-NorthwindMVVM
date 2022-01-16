namespace MvvmTools;

public static class NotifyPropertyChanged
{
    public static NotifyExpectation<T> ShouldNotifyOn<T, TProperty>(this T owner, Expression<Func<T, TProperty>> propertyPicker)
        where T : INotifyPropertyChanged 
        => CreateExpectation(owner, propertyPicker, true);

    public static NotifyExpectation<T> ShouldNotNotifyOn<T, TProperty>(this T owner, Expression<Func<T, TProperty>> propertyPicker) 
        where T : INotifyPropertyChanged
        => CreateExpectation(owner, propertyPicker, false);

    private static NotifyExpectation<T> CreateExpectation<T, TProperty>(T owner,
        Expression<Func<T, TProperty>> pickProperty,
        bool eventExpected) where T : INotifyPropertyChanged
    {
        var propertyName = ((MemberExpression)pickProperty.Body).Member.Name;
        return new NotifyExpectation<T>(owner, propertyName, eventExpected);
    }
}