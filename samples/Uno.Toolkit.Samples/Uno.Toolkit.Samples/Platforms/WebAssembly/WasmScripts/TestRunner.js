class SampleRunner {

	static async init() {

		if (!SampleRunner._navBackFromNestedPage) {
			const samplesAppExports = await Module.getAssemblyExports("Uno.Toolkit.Samples");

			SampleRunner._navBackFromNestedPage = samplesAppExports.Uno.Toolkit.Samples.App.NavBackFromNestedPage;
			SampleRunner._forceNavigation = samplesAppExports.Uno.Toolkit.Samples.App.ForceNavigation;
			SampleRunner._navigateToNestedSample = samplesAppExports.Uno.Toolkit.Samples.App.NavigateToNestedSample;
			SampleRunner._exitNestedSample = samplesAppExports.Uno.Toolkit.Samples.App.ExitNestedSample;
			SampleRunner._getDisplayScreenScaling = samplesAppExports.Uno.Toolkit.Samples.App.GetDisplayScreenScaling;
		}
	}

	static async NavBackFromNestedPage(unused) {
		await SampleRunner.init();
		return SampleRunner._navBackFromNestedPage();
	}

	static async ForceNavigation(sampleName) {
		await SampleRunner.init();
		return SampleRunner._forceNavigation(sampleName);
	}

	static async ExitNestedSample(unused) {
		await SampleRunner.init();
		return SampleRunner._exitNestedSample();
	}

	static async NavigateToNestedSample(sampleName) {
		await SampleRunner.init();
		return SampleRunner._navigateToNestedSample(sampleName);
	}

	static async GetDisplayScreenScaling(scaleValue) {
		await SampleRunner.init();
		return SampleRunner._getDisplayScreenScaling(scaleValue);
	}
}

globalThis.SampleRunner = SampleRunner;
