import { Routes, RouterModule } from '@angular/router';

import { BasketComponent } from './basket/basket.component';
import { CatalogComponent } from './catalog/catalog.component';

export const routes: Routes = [
    { path: '', redirectTo: 'catalog', pathMatch: 'full' },
    { path: 'basket', component: BasketComponent },
    { path: 'catalog', component: CatalogComponent }
   //Lazy async modules (angular-loader-router) and enable a router in each module. 
   //{
   // path: 'basket',  loadChildren: '/basket/basket.module' });
   // })
   //}
];

export const routing = RouterModule.forRoot(routes);
