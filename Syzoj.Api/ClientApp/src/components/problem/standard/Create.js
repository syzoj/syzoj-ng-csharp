import React, { Component } from 'react'
import Button from 'react-bootstrap/lib/Button'
import { Redirect } from 'react-router-dom'
import { request } from '../../../util';
import { ErrorList } from '../../Error'

export default class Create extends Component {
    constructor(props) {
        super(props)
        this.state = {errors: []}
        this.problem = {Statement: null}
    }

    doCreate() {
        request("/api/problem-standard/create", "post", {}).then(v => {
            this.setState({redirect: "/problem/standard/" + v + "/view"})
        }).catch(e => {
            this.setState({errors: e})
        })
    }

    render() {
        if(this.state.redirect)
            return <Redirect to={this.state.redirect} />
        else
            return [<ErrorList key="ErrorList" errors={this.state.errors} />, <Button key="Button" onClick={() => this.doCreate()}>创建</Button>]
    }
}