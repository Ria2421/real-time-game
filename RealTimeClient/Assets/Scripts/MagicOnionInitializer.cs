using MagicOnion.Client;
using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;

/// <summary>
/// MagicOnion騾包ｽｨ郢ｧ�､郢晢ｽｳ郢ｧ�ｿ郢晁ｼ斐♂郢晢ｽｼ郢ｧ�ｹ邵ｺ�ｮ郢ｧ�ｳ郢晢ｽｼ郢晁��蜃ｽ隰倹
/// </summary>
[MagicOnionClientGeneration(typeof(Shared.Interfaces.Services.IMyFirstService))]
partial class MagicOnionInitializer
{
    /// <summary>
    /// Resolver邵ｺ�ｮ騾具ｽｻ鬪ｭ�ｲ陷��ｽｦ騾��
    /// </summary>
    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterResolvers()
    {
        StaticCompositeResolver.Instance.Register(
            MagicOnionInitializer.Resolver,
            GeneratedResolver.Instance,
            BuiltinResolver.Instance,
            UnityResolver.Instance,
            PrimitiveObjectResolver.Instance
        );

        MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions
            .WithResolver(StaticCompositeResolver.Instance);
    }
}