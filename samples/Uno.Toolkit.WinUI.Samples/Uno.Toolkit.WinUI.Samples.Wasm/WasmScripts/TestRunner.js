class SampleRunner {

    static init() {

        if (!this._exitNestedSample) {
            this._forceNavigation = this.getMethod("[Uno.Toolkit.WinUI.Samples.Wasm] Uno.Toolkit.Samples.App:ForceNavigation");
            this._navBackFromNestedPage = this.getMethod("[Uno.Toolkit.WinUI.Samples.Wasm] Uno.Toolkit.Samples.App:NavBackFromNestedPage");
            this._exitNestedSample = this.getMethod("[Uno.Toolkit.WinUI.Samples.Wasm] Uno.Toolkit.Samples.App:ExitNestedSample");
            this._navigateToNestedSample = this.getMethod("[Uno.Toolkit.WinUI.Samples.Wasm] Uno.Toolkit.Samples.App:NavigateToNestedSample");
            this._getDisplayScreenScaling = this.getMethod("[Uno.Toolkit.WinUI.Samples.Wasm] Uno.Toolkit.Samples.App:GetDisplayScreenScaling");
        }
    }

    static getMethod(methodName) {
        var method = Module.mono_bind_static_method(methodName);

        if (!method) {
            throw new `Method ${methodName} does not exist`;
        }

        return method;
    }

    static NavBackFromNestedPage() {
        SampleRunner.init();
        return this._navBackFromNestedPage();
    }

    static ForceNavigation(sample) {
        SampleRunner.init();
        return this._forceNavigation(sample);
    } 

    static ExitNestedSample() {
        SampleRunner.init();
        return this._exitNestedSample();
    }

    static NavigateToNestedSample(pageName) {
        SampleRunner.init();
        return this._navigateToNestedSample(pageName);
    }

    static GetDisplayScreenScaling(value) {
        SampleRunner.init();
        return this._getDisplayScreenScaling(value);
    }
}
