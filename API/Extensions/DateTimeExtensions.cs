namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalcuateAge(this DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            if (dob > today)
            {
                throw new ArgumentException("Ngày sinh không thể ở trong tương lai.", nameof(dob));
            }

            var age = today.Year - dob.Year;

            if (dob > today.AddYears(-age))
            {
                age--;

                // Kiểm tra sinh nhật năm nhuận
                if (dob.Month == today.Month && dob.Day == today.Day && DateTime.IsLeapYear(today.Year))
                {
                    // Năm nay chưa có sinh nhật nhưng là năm nhuận
                    // Coi người đó lớn hơn một tuổi nếu sinh nhật của họ sau ngày hôm nay
                    age++;
                }
            }

            return age;
        }
    }
}
