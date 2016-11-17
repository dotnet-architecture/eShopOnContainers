import { Routes, RouterModule } from '@angular/router';

import { CatalogComponent } from './catalog.component';

const routes: Routes = [
    { path: 'catalog', component: CatalogComponent }
];

export const routing = RouterModule.forChild(routes);
