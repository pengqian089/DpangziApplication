namespace Dpz.Client.Enum;

[Flags]
public enum Permissions
{
    /// <summary>
    /// 没有权限
    /// </summary>
    None = 0,

    /// <summary>
    /// 系统权限
    /// </summary>
    System = 1 << 1,

    /// <summary>
    /// 普通用户
    /// </summary>
    Member = 1 << 2
}