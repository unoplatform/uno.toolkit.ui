using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
internal class XamlHelperTests
{
	[TestMethod]
	public void Complex_Test()
	{
		var result = XamlHelper.XamlAutoFill("""
			<DataTemplate>
				<StackPanel>
					<Grid>
						<!-- test -->
						<Button />
						<TextBlock>
						<TextBlock>
					<Grid Background="SkyBlue"
						  Tag="this unclosed node spans on multiple lines">
						<Button>
							<TextBlock>
						<Button Tag="this one has closing">
							<TextBlock>
						</Button>
					<Grid Tag="single-line multi-nesting"><Grid><Grid Tag="multi-line"
						Background="Pink">
						<Button>
					<Grid Tag="self-closing tag, should not have have closing tag appended"/>
					<Button />
				<Grid><Border><Grid>
					<Button Content="ThisShouldStillWork" />
				</Grid></Border></Grid>
				<GridA><Border><GridB>
		""").TrimEnd();
		var expectation = """
			<DataTemplate>
				<StackPanel>
					<Grid>
						<!-- test -->
						<Button />
						<TextBlock>
						</TextBlock>
						<TextBlock>
						</TextBlock>
					</Grid>
					<Grid Background="SkyBlue"
						  Tag="this unclosed node spans on multiple lines">
						<Button>
							<TextBlock>
							</TextBlock>
						</Button>
						<Button Tag="this one has closing">
							<TextBlock>
							</TextBlock>
						</Button>
					</Grid>
					<Grid Tag="single-line multi-nesting"><Grid><Grid Tag="multi-line"
						Background="Pink">
						<Button>
						</Button>
					</Grid>
					</Grid>
					</Grid>
					<Grid Tag="self-closing tag, should not have have closing tag appended"/>
					<Button />
				</StackPanel>
				<Grid><Border><Grid>
					<Button Content="ThisShouldStillWork" />
				</Grid></Border></Grid>
				<GridA><Border><GridB>
				</GridB>
				</Border>
				</GridA>
			</DataTemplate>
		""".TrimEnd();

		Assert.AreEqual(expectation, result);
	}
}
