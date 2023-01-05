namespace RezUtility.Utilities;

public static class TiengViet
{
	private static readonly string[] BangChu =
	{
		"aAeEoOuUiIdDyY",
		"áàạảãâấầậẩẫăắằặẳẵ",
		"ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
		"éèẹẻẽêếềệểễ",
		"ÉÈẸẺẼÊẾỀỆỂỄ",
		"óòọỏõôốồộổỗơớờợởỡ",
		"ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
		"úùụủũưứừựửữ",
		"ÚÙỤỦŨƯỨỪỰỬỮ",
		"íìịỉĩ",
		"ÍÌỊỈĨ",
		"đ",
		"Đ",
		"ýỳỵỷỹ",
		"ÝỲỴỶỸ"
	};

	public static string LoaiBoDau(this string xau)
	{
		for (int i = 1; i < BangChu.Length; i++)
		for (int j = 0; j < BangChu[i].Length; j++)
			xau = xau.Replace(BangChu[i][j], BangChu[0][i - 1]);
		return xau;
	}
}