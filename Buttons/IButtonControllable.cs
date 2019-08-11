/// <summary>
/// Objects that can be controlled with the press of a button
/// </summary>
public interface IButtonControllable
{
    void OnButtonPressed();
    void OnButtonReleased();
}