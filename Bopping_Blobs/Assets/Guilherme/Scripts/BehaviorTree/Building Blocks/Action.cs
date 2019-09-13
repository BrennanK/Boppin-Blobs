namespace BehaviorTree {
    /// <summary>
    /// An Action is a leaf node that have the responsibility of accessing information from the world and making changes to the world.
    /// When an action succeeds in making a change in the world, it returns EReturnStatus.SUCCESS, otherwise it returns EReturnStatus.FAILURE
    /// </summary>
    public abstract class Action : Behavior {

    }
}
