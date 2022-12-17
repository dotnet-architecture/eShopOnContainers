import {IOrderItem} from './orderItem.model';

export interface IOrderDetail {
    ordernumber: string;
    status: string;
    description: string;
    street: string;
    date: Date;
    city: number;
    state: string;
    zipcode: string;
    country: number;
    subtotal: number;
    coupon: string;
    discount: number;
    total: number;
    orderitems: IOrderItem[];
}
