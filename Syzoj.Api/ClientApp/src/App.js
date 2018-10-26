import React, { Component } from 'react'
import { Route } from 'react-router'
import { Layout } from './components/Layout'
import { Home } from './components/Home'
import { FetchData } from './components/FetchData'
import { Counter } from './components/Counter'
import { Login } from './pages/Login'
import { Register } from './pages/Register'
import { Create } from './pages/problem/Create'
import StandardProblemView from './pages/problem/standard/View'
import StandardProblemsetList from './pages/problemset/standard/List'
import StandardProblemsetView from './pages/problemset/standard/View'

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/counter' component={Counter} />
        <Route path='/fetchdata' component={FetchData} />
        <Route path='/register' component={Register} />
        <Route path='/login' component={Login} />
        <Route path='/problem/create' component={Create} />
        <Route path='/problem/standard/:id/view' component={StandardProblemView} />
        <Route path='/problemset-standard/list' component={StandardProblemsetList} />
        <Route path='/problemset-standard/view/:id' component={StandardProblemsetView} />
      </Layout>
    );
  }
}
