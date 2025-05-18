namespace DotnetProjectManagement.ProjectManagement.UseCases.DTOs;

public record PageRequest
{
    public PageRequest(int number, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(number);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        this.Number = number;
        this.Size = size;
    }

    public int Number { get; }

    public int Size { get; }

    public int Offset => this.Size * this.Number;
}
