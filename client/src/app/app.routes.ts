import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { adminGuard } from './guards/admin.guard';

// Only the home page ships in the initial bundle; every other page is
// downloaded the first time someone opens it.
export const routes: Routes = [
  { path: '', component: Home },
  { path: 'products', loadComponent: () => import('./component/products-page/products-page').then(m => m.ProductsPageComponent) },
  { path: 'cart', loadComponent: () => import('./component/cart/cart').then(m => m.Cart) },
  { path: 'sign-in', loadComponent: () => import('./component/sign-in/sign-in').then(m => m.SignIn) },
  { path: 'sign-up', loadComponent: () => import('./component/sign-up/sign-up').then(m => m.SignUp) },
  { path: 'user-profile', loadComponent: () => import('./component/user-profile/user-profile').then(m => m.UserProfile) },
  { path: 'thank-you', loadComponent: () => import('./component/thank-you/thank-you').then(m => m.ThankYou) },
  { path: 'dream-vacation', loadComponent: () => import('./component/dream-vacation/dream-vacation.component').then(m => m.DreamVacationComponent) },
  {
    path: 'admin',
    loadComponent: () => import('./component/admin-layout/admin-layout').then(m => m.AdminLayout),
    canActivate: [adminGuard],
    children: [
      { path: '', loadComponent: () => import('./component/admin-dashboard/admin-dashboard').then(m => m.AdminDashboard) },
      { path: 'products', loadComponent: () => import('./component/admin-product-list/admin-product-list').then(m => m.AdminProductList) },
      { path: 'categories', loadComponent: () => import('./component/admin-category/admin-category').then(m => m.AdminCategory) },
      { path: 'orders', loadComponent: () => import('./component/admin-orders/admin-orders').then(m => m.AdminOrders) }
    ]
  },
  { path: '**', redirectTo: '' }
];
