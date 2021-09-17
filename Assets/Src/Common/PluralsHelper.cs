public class PluralsHelper
{
    public static int GetPlural(int number)
    {
        var operatatableNumber = number % 100;
        if (operatatableNumber >= 5 && operatatableNumber <= 20)
        {
            return 3;
        }

        operatatableNumber %= 10;
        if (operatatableNumber == 1)
        {
            return 1;
        }
        else if (operatatableNumber >= 2 && operatatableNumber <= 4)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }
}
