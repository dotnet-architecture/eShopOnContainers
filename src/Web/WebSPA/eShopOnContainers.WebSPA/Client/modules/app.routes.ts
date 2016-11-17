import { Routes, RouterModule } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'catalog', pathMatch: 'full' }
  // Lazy async modules
  // {
  //  path: 'login', loadChildren: () => new Promise(resolve => {
  //    (require as any).ensure([], (require: any) => {
  //      resolve(require('./+login/login.module').LoginModule);
  //    });
  //  })
  // }
];

export const routing = RouterModule.forRoot(routes);
