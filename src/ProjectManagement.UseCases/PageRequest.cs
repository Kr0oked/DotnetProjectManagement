namespace DotnetProjectManagement.ProjectManagement.UseCases;

public record PageRequest
{
    public int Number { get; }

    public int Size { get; }

    public int Offset => this.Size * this.Number;

    public PageRequest(int number, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(number);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        this.Number = number;
        this.Size = size;
    }
}
