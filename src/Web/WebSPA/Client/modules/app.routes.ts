import { Routes, RouterModule } from '@angular/router';

import { BasketComponent } from './basket/basket.component';
import { CatalogComponent } from './catalog/catalog.component';
import { OrdersComponent } from './orders/orders.component';
import { OrdersDetailComponent } from './orders/orders-detail/orders-detail.component';
import { OrdersNewComponent } from './orders/orders-new/orders-new.component';
import { CampaignsComponent } from './campaigns/campaigns.component';
import { CampaignsDetailComponent } from './campaigns/campaigns-detail/campaigns-detail.component';

export const routes: Routes = [
    { path: '', redirectTo: 'catalog', pathMatch: 'full' },
    { path: 'basket', component: BasketComponent },
    { path: 'catalog', component: CatalogComponent },
    { path: 'orders', component: OrdersComponent },
    { path: 'orders/:id', component: OrdersDetailComponent },
    { path: 'order', component: OrdersNewComponent },
    { path: 'campaigns', component: CampaignsComponent },
    { path: 'campaigns/:id', component: CampaignsDetailComponent }
];

export const routing = RouterModule.forRoot(routes);
