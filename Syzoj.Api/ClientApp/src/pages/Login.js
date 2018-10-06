import React, { Component } from 'react'
import Form from 'react-bootstrap/lib/Form'
import FormGroup from 'react-bootstrap/lib/FormGroup'
import FormControl from 'react-bootstrap/lib/FormControl'
import ControlLabel from 'react-bootstrap/lib/ControlLabel'
import Button from 'react-bootstrap/lib/Button'
import { request } from '../util'
import { ErrorList } from '../components/Error'
import { Redirect } from 'react-router-dom'

export class Login extends Component {
    constructor(props) {
        super(props)
        this.state = {userName: '', password: '', isPersistent: false, errors: null, success: false}
    }
    submit(e) {
        e.preventDefault()
        request("/api/auth/login", "post", {
            UserName: this.state.userName,
            Password: this.state.password,
            IsPersistent: this.state.isPersistent,
        }).then((response) => {
            console.log(response)
            this.setState({success: true})
        }).catch(e => {
            this.setState({errors: e})
        })
    }
    render() {
        if(this.state.success)
            return <Redirect to='/' />
        return <div>
            <ErrorList errors={this.state.errors} />
            <Form onSubmit={(e) => this.submit(e)}>
                <FormGroup controlId="formBasicUserName">
                    <ControlLabel>User Name</ControlLabel>
                    <FormControl placeholder="Enter user name" onChange={(e) => this.setState({userName: e.target.value})} value={this.state.userName} />
                </FormGroup>
                <FormGroup controlId="formBasicPassword">
                    <ControlLabel>Password</ControlLabel>
                    <FormControl type="password" placeholder="Password" onChange={(e) => this.setState({password: e.target.value})} value={this.state.password} />
                </FormGroup>
                <FormGroup id="formBasicChecbox">
                    <ControlLabel>Remember me</ControlLabel>
                    <FormControl type="checkbox"  />
                </FormGroup>
                <Button variant="primary" type="submit">
                    Submit
                </Button>
            </Form>
        </div>;
    }
}