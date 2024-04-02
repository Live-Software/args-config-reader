namespace ArgsConfigReader; 

public static class EnumerableExtensions {
    public static IEnumerable<T> NonNull<T>(this IEnumerable<T?> list) {
        return list.Where(obj => obj != null)
            .Select(obj => obj!);
    }
}