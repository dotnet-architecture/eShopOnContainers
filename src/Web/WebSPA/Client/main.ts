import './polyfills';

import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { AppModule }              from './modules/app.module';

if (process.env.ENV === 'Development') {
  // Development
} else {
  // Production
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule);

// Basic hot reloading support. Automatically reloads and restarts the Angular 2 app each time
// you modify source files. This will not preserve any application state other than the URL.
declare var module: any;

if (module.hot) {
    module.hot.accept();
}
