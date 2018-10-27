import React, { Component } from 'react'
import { Route } from 'react-router'
import { Layout } from './components/Layout'
import { Home } from './components/Home'
import { FetchData } from './components/FetchData'
import { Counter } from './components/Counter'
import { Login } from './pages/Login'
import { Register } from './pages/Register'
import StandardProblemsetList from './pages/ProblemList'
import StandardProblemsetView from './pages/ProblemView'
import StandardProblemsetSubmissionView from './pages/SubmissionView'

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
        <Route path='/problems' component={StandardProblemsetList} />
        <Route path='/problem/:id' component={StandardProblemsetView} />
        <Route path='/submission/:id' component={StandardProblemsetSubmissionView} />
      </Layout>
    );
  }
}
