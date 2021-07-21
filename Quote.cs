namespace oed
{
	public class Quote
	{
		public int Year { get; set; }
		public string Text { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
	

		public Quote()
		{
			this.Year = 0;
			this.Text = "";
			this.Title = "";
			this.Author = "";
		}

		public Quote(Quote other)
		{
			Year = other.Year;
			Text = other.Text;
			Title = other.Title;
			Author = other.Author;
		}
	}
}
