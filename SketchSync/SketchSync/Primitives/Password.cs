namespace SketchSync.Primitives;

public record Password(byte[] Hash, byte[] Salt);