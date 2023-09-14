namespace EventLib; 

public static class L {
    public static void og(object msg) {
        Main.entry.Logger.Log(msg?.ToString() ?? "null");
    }
}
