import React, { Component } from 'react';
import { MarkdownLatex } from './MarkdownLatex';
import AceEditor from 'react-ace';
import { Col, Grid, Row, ListGroup, ListGroupItem } from 'react-bootstrap';
import 'brace/mode/c_cpp';

export class Problem extends Component {
  displayName = Problem.name

  constructor(props) {
    super(props);
    this.state = { statement: null, loading: true };
  }

  componentDidMount()
  {
    fetch('api/problemset/1/problem/' + this.props.match.params.id)
      .then(response => response.json())
      .then(data => {
        this.setState({ statement: data, loading: false });
      });
  }

  renderMarkdown(name, content)
  {
    if(content != null && content !== "")
    {
      return <div>
        <h2>{name}</h2>
        <MarkdownLatex source={content} />
      </div>;
    }
    else
    {
      return null;
    }
  }

  setLanguage(language)
  {
    this.setState({language: language});
  }

  render() {
    return this.state.loading ?
        <p>Loading...</p>
    :
    <Grid>
      <Row>
        <Col xs="12" sm="12">
          <div className="page-header text-center">
            <h1>#{this.state.statement.ProblemsetProblemId}. {this.state.statement.Title}</h1>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs="12" sm="12">
          {this.renderMarkdown("题目描述", this.state.statement.Content.Description)}
          {this.renderMarkdown("输入格式", this.state.statement.Content.InputFormat)}
          {this.renderMarkdown("输出格式", this.state.statement.Content.OutputFormat)}
          {this.renderMarkdown("样例", this.state.statement.Content.Example)}
          {this.renderMarkdown("数据范围与提示", this.state.statement.Content.LimitsAndHints)}
        </Col>
      </Row>
      <Row>
        <Col xs="4" sm="4">
          <ListGroup>
            <ListGroupItem active={this.state.language == 'cpp'} onClick={(props) => this.setLanguage('cpp')}>C++</ListGroupItem>
            <ListGroupItem active={this.state.language == 'c'} onClick={(props) => this.setLanguage('c')}>C</ListGroupItem>
            <ListGroupItem active={this.state.language == 'pascal'} onClick={(props) => this.setLanguage('pascal')}>Pascal</ListGroupItem>
          </ListGroup>
        </Col>
        <Col xs="12" sm="8">
          <AceEditor mode="c_cpp" name="code" width="" fontSize={16} />
        </Col>
      </Row>
    </Grid>;
  }
}