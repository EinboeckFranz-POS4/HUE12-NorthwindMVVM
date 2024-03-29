﻿namespace MvvmTools;

public static class ExtensionMethods
{
    public static ObservableCollection<T> AsObservableCollection<T>(this IEnumerable<T> data)
    {
        var observableCollection = new ObservableCollection<T>();
        foreach (var item in data)
            observableCollection.Add(item);

        return observableCollection;
    }
}