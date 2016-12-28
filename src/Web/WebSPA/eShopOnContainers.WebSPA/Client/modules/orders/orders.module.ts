import { NgModule }             from '@angular/core';
import { BrowserModule  }       from '@angular/platform-browser';

import { SharedModule }         from '../shared/shared.module';
import { OrdersComponent }      from './orders.component';
import { OrdersDetailComponent }      from './orders-detail/orders-detail.component';
import { OrdersNewComponent }      from './orders-new/orders-new.component';
import { OrdersService }        from './orders.service';
import { Pager }                from '../shared/components/pager/pager';

@NgModule({
    imports: [BrowserModule, SharedModule],
    declarations: [OrdersComponent, OrdersDetailComponent, OrdersNewComponent],
    providers: [OrdersService]
})
export class OrdersModule { }
